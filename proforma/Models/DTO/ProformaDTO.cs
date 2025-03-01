namespace proforma.Models.DTO
{
    public class ProformaDTO
    {
        public string InvoiceNumber { get; set; } = null!;

        public ulong? OrganizationId { get; set; }

        public ulong? ClientId { get; set; }

        public string? Comment { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? Taxes { get; set; }

        public decimal? DiscountTotal { get; set; }

        public decimal? Total { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
