namespace proforma.Models.DTO;

public class UserDTO
{
    public ulong Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool? Active { get; set; }
}

