using System;

namespace proforma.Models
{
    public partial class PrintingTemplate
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string TemplateName { get; set; } = null!;
        public string? PageOrientation { get; set; }
        public string? ColorMode { get; set; }
        public int? Copies { get; set; }
        public string? PaperSize { get; set; }
        public string? HeaderHtml { get; set; }
        public string? FooterHtml { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
