using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services
{
    public class AuthService
    {
        private readonly ProformaContext _context;
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ProformaContext context, IConfiguration configuration, JwtService jwtService, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Valida las credenciales, genera el access token y crea una sesión con refresh token.
        /// </summary>
        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto, string fingerprint)
        {
            // Buscar el usuario por correo
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            // Verificar existencia del usuario y la contraseña
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                _logger.LogWarning("Fallo de autenticación para el usuario {Email}.", loginDto.Email);
                return null; // Credenciales incorrectas
            }

            // Generar access token y refresh token
            var accessToken = _jwtService.GenerateJwtToken(user.Id, user.Email);
            var refreshToken = await CreateSessionAsync(user.Id, fingerprint);

            _logger.LogInformation("Usuario {Email} autenticado exitosamente.", user.Email);
            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Active = user.Active
                }
            };
        }

        /// <summary>
        /// Crea una sesión en la base de datos generando un refresh token dividido en selector y validador.
        /// Se almacena en la BD en el formato "selector:hashedValidator" y se retorna al cliente en el formato "selector.validator".
        /// </summary>
        public async Task<string> CreateSessionAsync(ulong userId, string fingerprint)
        {
            var selector = Guid.NewGuid().ToString("N");
            var validator = Guid.NewGuid().ToString("N");
            var refreshToken = $"{selector}.{validator}";
            var hashedValidator = BCrypt.Net.BCrypt.HashPassword(validator);
            var storedToken = $"{selector}:{hashedValidator}";

            var session = new Usersession
            {
                UserId = userId,
                RefreshToken = storedToken,
                Fingerprint = fingerprint,
                ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtSettings:RefreshTokenExpiryInDays"]))
            };

            _context.Usersessions.Add(session);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        /// <summary>
        /// Valida el refresh token recibido y, si es correcto, revoca la sesión actual,
        /// genera un nuevo access token y crea una nueva sesión (rotación del refresh token).
        /// </summary>
        public async Task<(string accessToken, string refreshToken)?> RefreshSessionAsync(string providedRefreshToken, string fingerprint)
        {
            var parts = providedRefreshToken.Split('.');
            if (parts.Length != 2)
            {
                _logger.LogWarning("Formato de refresh token inválido.");
                return null;
            }
            var selector = parts[0];
            var validator = parts[1];

            var session = await _context.Usersessions.FirstOrDefaultAsync(s => s.RefreshToken.StartsWith(selector + ":") && s.RevokedAt == null);
            if (session == null)
            {
                _logger.LogWarning("No se encontró sesión activa para el selector {Selector}.", selector);
                return null;
            }

            if (session.Fingerprint != fingerprint)
            {
                _logger.LogWarning("Fingerprint no coincide para la sesión {SessionId}.", session.Id);
                return null;
            }

            if (session.ExpiryDate <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expirado para la sesión {SessionId}.", session.Id);
                return null;
            }

            var storedParts = session.RefreshToken.Split(':');
            if (storedParts.Length != 2)
            {
                _logger.LogError("Formato de refresh token almacenado inválido para la sesión {SessionId}.", session.Id);
                return null;
            }
            var hashedValidator = storedParts[1];
            if (!BCrypt.Net.BCrypt.Verify(validator, hashedValidator))
            {
                _logger.LogWarning("Validador de refresh token no coincide para la sesión {SessionId}.", session.Id);
                return null;
            }

            // Revocar la sesión actual
            session.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(session.UserId);
            if (user == null)
            {
                _logger.LogError("Usuario no encontrado para la sesión {SessionId}.", session.Id);
                return null;
            }

            var newAccessToken = _jwtService.GenerateJwtToken(user.Id, user.Email);
            var newRefreshToken = await CreateSessionAsync(user.Id, fingerprint);

            _logger.LogInformation("Refresh token rotado exitosamente para el usuario {Email}.", user.Email);
            return (newAccessToken, newRefreshToken);
        }

        /// <summary>
        /// Revoca (cierra) la sesión asociada al refresh token recibido.
        /// </summary>
        public async Task<bool> RevokeSessionAsync(string providedRefreshToken, string fingerprint)
        {
            var parts = providedRefreshToken.Split('.');
            if (parts.Length != 2)
            {
                _logger.LogWarning("Formato de refresh token inválido en logout.");
                return false;
            }
            var selector = parts[0];
            var validator = parts[1];

            var session = await _context.Usersessions.FirstOrDefaultAsync(s => s.RefreshToken.StartsWith(selector + ":") && s.RevokedAt == null);
            if (session == null)
            {
                _logger.LogWarning("No se encontró sesión activa para logout con selector {Selector}.", selector);
                return false;
            }

            if (session.Fingerprint != fingerprint)
            {
                _logger.LogWarning("Fingerprint no coincide en logout para la sesión {SessionId}.", session.Id);
                return false;
            }

            var storedParts = session.RefreshToken.Split(':');
            if (storedParts.Length != 2)
            {
                _logger.LogError("Formato de refresh token almacenado inválido en logout para la sesión {SessionId}.", session.Id);
                return false;
            }
            var hashedValidator = storedParts[1];
            if (!BCrypt.Net.BCrypt.Verify(validator, hashedValidator))
            {
                _logger.LogWarning("Validador de refresh token no coincide en logout para la sesión {SessionId}.", session.Id);
                return false;
            }

            session.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Sesión {SessionId} revocada exitosamente.", session.Id);
            return true;
        }

        /// <summary>
        /// Solicita la recuperación de contraseña generando un token y almacenándolo en la tabla password_resets.
        /// Además, envía un correo con el token al usuario.
        /// </summary>
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            // Buscar el usuario por correo
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("No se encontró usuario con el correo {Email}", email);
                return false;
            }

            // Generar un token único para la recuperación
            var token = Guid.NewGuid().ToString("N");

            // Crear un registro en la tabla password_resets
            var passwordReset = new PasswordReset
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Token de recuperación para {Email}: {Token}", email, token);

            // Enviar el correo de recuperación
            try
            {
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"]);
                var smtpUsername = _configuration["Smtp:Username"];
                var smtpPassword = _configuration["Smtp:Password"];
                var fromEmail = _configuration["Smtp:FromEmail"];

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Recuperación de Contraseña",
                    Body = $"Su token de recuperación es: {token}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }

                _logger.LogInformation("Correo de recuperación enviado exitosamente a {Email}.", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando el correo de recuperación a {Email}.", email);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resetea la contraseña del usuario validando el token de recuperación.
        /// </summary>
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var resetRecord = await _context.PasswordResets.FirstOrDefaultAsync(pr => pr.Email == email && pr.Token == token);
            if (resetRecord == null)
            {
                _logger.LogWarning("Token de recuperación inválido para {Email}", email);
                return false;
            }

            // Verificar que el token no haya expirado (1 hora de vigencia)
            if (resetRecord.CreatedAt.HasValue && resetRecord.CreatedAt.Value.AddHours(1) < DateTime.UtcNow)
            {
                _logger.LogWarning("El token de recuperación para {Email} ha expirado.", email);
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogError("No se encontró el usuario para el correo {Email}", email);
                return false;
            }

            // Actualizar la contraseña del usuario
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Eliminar el registro de password_resets
            _context.PasswordResets.Remove(resetRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contraseña actualizada correctamente para {Email}", email);
            return true;
        }
    }
}
