namespace proforma.Models.DTO
{
    public class SystemConfigurationDTO
    {

        public ulong Id { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
