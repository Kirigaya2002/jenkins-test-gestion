using System;

namespace proforma.Models
{
    public partial class SystemConfiguration
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string Key { get; set; } = null!;

        public string Value { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
