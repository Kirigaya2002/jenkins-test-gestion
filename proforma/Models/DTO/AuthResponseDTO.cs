namespace proforma.Models.DTO;

public class AuthResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public UserDTO User { get; set; } = null!;
}
