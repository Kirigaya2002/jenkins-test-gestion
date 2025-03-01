namespace proforma.Models.DTO
{
    public class UserSessionDTO
    {
        public string RefreshToken { get; set; } = null!;
        public string Fingerprint { get; set; } = null!;
    }
}
