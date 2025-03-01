using System.Text.Json.Serialization;

namespace proforma.Models
{
    public class InventoryDetail
    {
        public ulong Id { get; set; }
        public ulong InventoryId { get; set; }
        public ulong ArticleId { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Propiedades de navegación

        [JsonIgnore]
        public virtual Inventory Inventory { get; set; } = null!;


        public virtual Article Article { get; set; } = null!;
    }
}
