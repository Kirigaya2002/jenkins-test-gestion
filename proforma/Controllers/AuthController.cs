using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;
using System.Linq;
using System.Threading.Tasks;

namespace proforma.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly ProformaContext _context;

        public AuthController(AuthService authService, ILogger<AuthController> logger, ProformaContext context)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var fingerprint = Request.Headers["Fingerprint"].FirstOrDefault();
            if (string.IsNullOrEmpty(fingerprint))
            {
                _logger.LogWarning("Intento de login sin fingerprint.");
                return BadRequest("Fingerprint requerida.");
            }

            var authResponse = await _authService.LoginAsync(loginDto, fingerprint);
            if (authResponse == null)
            {
                _logger.LogWarning("Credenciales inválidas para el usuario {Email}.", loginDto.Email);
                return Unauthorized("Credenciales inválidas.");
            }

            SetRefreshTokenCookie(authResponse.RefreshToken);
            return Ok(new { AccessToken = authResponse.AccessToken, User = authResponse.User });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("No se encontró refreshToken en la cookie o está vacío.");
                    return Unauthorized("No hay refresh token.");
                }

                var fingerprint = Request.Headers["Fingerprint"].FirstOrDefault();
                if (string.IsNullOrEmpty(fingerprint))
                {
                    _logger.LogWarning("Intento de refresh sin fingerprint.");
                    return BadRequest("Fingerprint requerida.");
                }

                var result = await _authService.RefreshSessionAsync(refreshToken, fingerprint);
                if (result == null || string.IsNullOrEmpty(result.Value.accessToken) || string.IsNullOrEmpty(result.Value.refreshToken))
                {
                    _logger.LogWarning("Intento de refresco de token fallido: Token inválido o fingerprint incorrecta.");
                    return Unauthorized("Sesión inválida o fingerprint no coincide.");
                }

                var (accessToken, newRefreshToken) = result.Value;
                SetRefreshTokenCookie(newRefreshToken);
                return Ok(new { AccessToken = accessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en el endpoint RefreshToken.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                _logger.LogWarning("Intento de logout sin refreshToken.");
                return BadRequest("No hay refresh token.");
            }

            var fingerprint = Request.Headers["Fingerprint"].FirstOrDefault();
            if (string.IsNullOrEmpty(fingerprint))
            {
                _logger.LogWarning("Intento de logout sin fingerprint.");
                return BadRequest("Fingerprint requerida.");
            }

            var revoked = await _authService.RevokeSessionAsync(refreshToken, fingerprint);
            if (!revoked)
            {
                _logger.LogWarning("Intento de logout fallido.");
                return BadRequest("No se pudo cerrar la sesión.");
            }

            Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            _logger.LogInformation("Sesión cerrada exitosamente.");
            return Ok("Sesión cerrada exitosamente.");
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("No se encontró refreshToken en la cookie para /auth/me.");
                return Unauthorized("No hay sesión activa.");
            }

            var parts = refreshToken.Split('.');
            if (parts.Length != 2)
            {
                _logger.LogWarning("Formato de refresh token inválido en /auth/me.");
                return Unauthorized("Sesión inválida.");
            }
            var selector = parts[0];
            var validator = parts[1];

            var session = await _context.Usersessions
                .FirstOrDefaultAsync(s => s.RefreshToken.StartsWith(selector + ":") && s.RevokedAt == null);

            if (session == null)
            {
                _logger.LogWarning("Sesión no encontrada o revocada en /auth/me.");
                return Unauthorized("Sesión inválida.");
            }

            var fingerprint = Request.Headers["Fingerprint"].FirstOrDefault();
            if (string.IsNullOrEmpty(fingerprint) || session.Fingerprint != fingerprint)
            {
                _logger.LogWarning("Fingerprint no coincide en /auth/me para la sesión {SessionId}.", session.Id);
                return Unauthorized("Sesión inválida o fingerprint no coincide.");
            }

            if (session.ExpiryDate <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expirado para la sesión {SessionId} en /auth/me.", session.Id);
                return Unauthorized("Sesión expirada.");
            }

            var storedParts = session.RefreshToken.Split(':');
            if (storedParts.Length != 2)
            {
                _logger.LogError("Formato de refresh token almacenado inválido para la sesión {SessionId} en /auth/me.", session.Id);
                return Unauthorized("Sesión inválida.");
            }
            var hashedValidator = storedParts[1];
            if (!BCrypt.Net.BCrypt.Verify(validator, hashedValidator))
            {
                _logger.LogWarning("Validador de refresh token no coincide para la sesión {SessionId} en /auth/me.", session.Id);
                return Unauthorized("Sesión inválida.");
            }

            var user = await _context.Users.FindAsync(session.UserId);
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado para /auth/me en la sesión {SessionId}.", session.Id);
                return Unauthorized("Usuario no encontrado.");
            }

            var userDto = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Active = user.Active
            };

            return Ok(new { User = userDto });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RequestPasswordResetAsync(dto.Email);
            if (!result)
            {
                return BadRequest("No se pudo generar el token de recuperación. Verifica el correo.");
            }

            return Ok("Se ha enviado un correo con las instrucciones para recuperar la contraseña.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
            if (!result)
            {
                return BadRequest("El token es inválido, ha expirado o el usuario no existe.");
            }

            return Ok("Contraseña actualizada correctamente.");
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken, options);
        }
    }
}
