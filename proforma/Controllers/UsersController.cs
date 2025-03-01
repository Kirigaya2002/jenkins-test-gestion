using Microsoft.AspNetCore.Mvc;
using proforma.Services;
using proforma.Models.DTO;
using proforma.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace proforma.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDto)
    {
        if (await _userService.EmailExistsAsync(createUserDto.Email))
        {
            return BadRequest("El correo electrónico ya está registrado.");
        }

        var user = await _userService.CreateUserAsync(createUserDto);
        return Ok();
    }

    // listar paginacion de usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (users, totalPages) = await _userService.GetUsersAsync(page, pageSize);

        return Ok(new
        {
            users,
            totalPages,
            currentPage = page
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUserById(ulong id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        return Ok(user);
    }

    //[HttpGet("{id}/details")]
    //public async Task<ActionResult<User>> GetUserWithRelations(ulong id)
    //{
    //    var user = await _userService.GetUserWithRelationsAsync(id);
    //    if (user == null)
    //    {
    //        return NotFound(new { message = "Usuario no encontrado" });
    //    }
    //    return Ok(user);
    //}

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(ulong id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound("Usuario no encontrado o ya eliminado.");
        }

        return NoContent();
    }

    [HttpPut("{id}/restore")]
    public async Task<IActionResult> RestoreUser(ulong id)
    {
        var success = await _userService.RestoreUserAsync(id);
        if (!success)
        {
            return NotFound("Usuario no encontrado o no está eliminado.");
        }

        return NoContent();
    }

    //obtenenr listado paginado usuarios
    [HttpGet("deleted")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetDeletedUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (users, totalPages) = await _userService.GetDeletedUsersAsync(page, pageSize);

        return Ok(new
        {
            users,
            totalPages,
            currentPage = page
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(ulong id, [FromBody] UpdateUserDTO updateUserDto)
    {
        var success = await _userService.UpdateUserAsync(id, updateUserDto);
        if (!success)
        {
            return NotFound("Usuario no encontrado o eliminado.");
        }

        return NoContent();
    }

    [HttpGet("searchByName")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> SearchUsersByName([FromQuery] string name)
    {
        var users = await _userService.SearchUsersByNameAsync(name);
        return Ok(users);
    }
}
