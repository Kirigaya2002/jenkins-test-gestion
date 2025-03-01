using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace proforma.Models;

public partial class Article
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

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<ArticleBarcode> ArticleBarcodes { get; set; } = new List<ArticleBarcode>();

    //public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    [JsonIgnore]
    public virtual ICollection<InventoryDetail> InventoryDetails { get; set; } = new List<InventoryDetail>();
}
