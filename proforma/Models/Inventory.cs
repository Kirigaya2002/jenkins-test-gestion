using proforma.Models;
using System.Text.Json.Serialization;

public partial class Inventory
{

    //el resto de cosas de inventario viene en inventorydetail

    public ulong Id { get; set; }
    public ulong OrganizationId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<InventoryDetail> InventoryDetails { get; set; } = new List<InventoryDetail>();

    public static implicit operator ulong(Inventory v)
    {
        throw new NotImplementedException();
    }
}
