using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class InventoryDetailCreateDTO
    {
        [Range(1, long.MaxValue, ErrorMessage = "El artículo es obligatorio.")]
        public long ArticleId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Quantity { get; set; }

        public string? Notes { get; set; }
    }
}
