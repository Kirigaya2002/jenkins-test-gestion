using System;
using System.Collections.Generic;

namespace proforma.Models;

public partial class Usersession
{
    public ulong Id { get; set; }

    public ulong UserId { get; set; }

    public string RefreshToken { get; set; } = null!;

    public string Fingerprint { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
