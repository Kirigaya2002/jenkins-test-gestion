using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services;

public class UserService
{
    private readonly ProformaContext _context;
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public UserService(ProformaContext context, JwtService jwtService, IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    //Listar usuarios paginados (modificados)
    public async Task<(IEnumerable<UserDTO> Users, int TotalPages)> GetUsersAsync(int page, int pageSize)
    {
        var query = _context.Users
            .Where(u => u.DeletedAt == null); // Filtrar usuarios activos 

        int totalUsers = await query.CountAsync(); 
        int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        var users = await query
            .OrderBy(u => u.Name) 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Active = u.Active
            })
            .ToListAsync();

        return (users, totalPages);
    }


    public async Task<UserDTO?> GetUserByIdAsync(ulong id)
    {
        return await _context.Users
            .Where(u => u.Id == id && u.DeletedAt == null)
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Active = u.Active
            })
            .FirstOrDefaultAsync();
    }

    //Esto solo era de prueba 
    //public async Task<User?> GetUserWithRelationsAsync(ulong id)
    //{
    //    return await _context.Users
    //        .Include(u => u.SystemConfigurations)
    //        .Include(u => u.PrintingTemplates)
    //        //.Include(u => u.Usersessions)
    //        .FirstOrDefaultAsync(u => u.Id == id);
    //}

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> CreateUserAsync(CreateUserDTO createUserDto)
    {
        var user = new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password), // Encripta la contraseña
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Crear una configuración de sistema por defecto para el usuario creado
        var systemConfiguration = new SystemConfiguration
        {
            UserId = user.Id,
            Key = "theme",
            Value = "light",
            Description = "Configuración de tema",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Configurations.Add(systemConfiguration);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(ulong id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        if (user == null)
        {
            return false; // El usuario no existe o ya está eliminado
        }

        user.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RestoreUserAsync(ulong id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt != null);
        if (user == null)
        {
            return false;
        }

        user.DeletedAt = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    // Listar paginacion (modificado)
    public async Task<(IEnumerable<UserDTO> Users, int TotalPages)> GetDeletedUsersAsync(int page, int pageSize)
    {
        var query = _context.Users
            .Where(u => u.DeletedAt != null); // Filtrar usuarios eliminados

        int totalUsers = await query.CountAsync(); 
        int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        var users = await query
            .OrderBy(u => u.Name) 
            .Skip((page - 1) * pageSize) 
            .Take(pageSize) 
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Active = u.Active
            })
            .ToListAsync();

        return (users, totalPages);
    }

    public async Task<bool> UpdateUserAsync(ulong id, UpdateUserDTO updateUserDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        if (user == null)
        {
            return false;
        }

        user.Name = updateUserDto.Name;
        user.Email = updateUserDto.Email;
        user.Active = updateUserDto.Active;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserDTO>> SearchUsersByNameAsync(string searchText)
    {
        return await _context.Users
            .Where(u => u.DeletedAt == null &&
                 (EF.Functions.Like(u.Name.ToLower(), $"%{searchText.ToLower()}%") ||
                  EF.Functions.Like(u.Email.ToLower(), $"%{searchText.ToLower()}%")))
            .Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Active = u.Active
            })
            .ToListAsync();
    }
}
