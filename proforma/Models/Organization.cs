using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace proforma.Models;

public partial class Organization
{
    public ulong Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ManagerIdentification { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    //(nuevo relacion usuario)
    public ulong? UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [JsonIgnore]
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    //public virtual ICollection<PrintingTemplate> PrintingTemplates { get; set; } = new List<PrintingTemplate>();

    [JsonIgnore]
    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();

    [JsonIgnore]
    public virtual ICollection<OrganizationClient> OrganizationClients { get; set; } = new List<OrganizationClient>();
}
