namespace proforma.Models.DTO
{
    public class ClientDTO
    {
        public ulong Id { get; set; }

        public string Identification { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool? Active { get; set; }
    }
}
