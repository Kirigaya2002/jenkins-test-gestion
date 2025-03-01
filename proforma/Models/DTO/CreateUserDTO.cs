using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO;

public class CreateUserDTO
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password length must be between 6 and 100 characters.")]
    public string Password { get; set; } = null!;
}
