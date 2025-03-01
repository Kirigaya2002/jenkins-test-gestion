
namespace proforma.Models.DTO
{
    public class ArticleDTO
    {
        public ulong Id { get; set; }
        public string? SourceInfo { get; set; }
        public string Description { get; set; } = null!;
        public decimal TaxPercentage { get; set; }
        public decimal Cost { get; set; }
        public decimal ProfitPercentage { get; set; }
        public decimal? NetProfit { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? TaxNet { get; set; }
        public decimal? TaxPercentual { get; set; }
        public decimal? Total { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
