using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace proforma.Models;

public partial class ArticleBarcode
{
    public ulong Id { get; set; }

    public ulong ArticleId { get; set; }

    public string Barcode { get; set; } = null!;

    public virtual Article Article { get; set; } = null!;
}
