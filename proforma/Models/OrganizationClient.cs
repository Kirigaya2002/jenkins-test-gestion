namespace proforma.Models;

public partial class OrganizationClient
{
    public ulong ClientId { get; set; }
    public ulong OrganizationId { get; set; }
    public virtual Client Client { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }
    public virtual Organization? Organization { get; set; }
}
