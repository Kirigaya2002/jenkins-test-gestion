using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace proforma.Models;

public partial class ProformaDetail
{
    public ulong Id { get; set; }

    public ulong ProformaId { get; set; }

    public string ArticleCode { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal? Discount { get; set; }

    public string? ItemComment { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? UnitTax { get; set; }

    public decimal? LineTotal { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public virtual Proforma Proforma { get; set; } = null!;
}
