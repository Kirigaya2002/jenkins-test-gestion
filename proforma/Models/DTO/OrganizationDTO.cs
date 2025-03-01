namespace proforma.Models.DTO
{
    public class OrganizationDTO
    {
        public ulong Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ManagerIdentification { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public ulong? UserId { get; set; }
    }
}
