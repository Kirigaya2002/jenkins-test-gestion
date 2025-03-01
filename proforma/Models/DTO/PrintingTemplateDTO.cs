namespace proforma.Models.DTO
{
    public class PrintingTemplateDTO
    {
        public ulong UserId { get; set; }

        public string TemplateName { get; set; } = null!;

        public string? PageOrientation { get; set; }

        public string? ColorMode { get; set; }

        public int? Copies { get; set; }

        public string? PaperSize { get; set; }

        public string? HeaderHtml { get; set; }

        public string? FooterHtml { get; set; }
    }
}
