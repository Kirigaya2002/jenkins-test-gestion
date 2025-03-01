using System;
using System.Collections.Generic;

namespace proforma.Models;

public partial class Proforma
{
    public ulong Id { get; set; }

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

    public virtual Client? Client { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<ProformaDetail> ProformaDetails { get; set; } = new List<ProformaDetail>();
}
