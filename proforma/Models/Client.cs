using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace proforma.Models;

public partial class Client
{
    public ulong Id { get; set; }

    public string Identification { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();

    [JsonIgnore]
    public ICollection<OrganizationClient> OrganizationClients { get; set; } = new List<OrganizationClient>();
}
