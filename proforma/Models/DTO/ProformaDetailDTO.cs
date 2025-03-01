namespace proforma.Models.DTO
{
    public class ProformaDetailDTO
    {
        public ulong ProformaId { get; set; }

        public string ArticleCode { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal? Discount { get; set; }

        public string? ItemComment { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? UnitTax { get; set; }

        public decimal? LineTotal { get; set; }
    }
}
