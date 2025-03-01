using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class InventoryUpdateDTO
    {
        [Range(1, long.MaxValue, ErrorMessage = "La organización es obligatoria.")]
        public long OrganizationId { get; set; }

        // Si se desea actualizar los detalles junto con el inventario:
        public List<InventoryDetailCreateDTO>? Details { get; set; }
    }
}
