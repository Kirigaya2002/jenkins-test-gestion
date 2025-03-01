using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class InventoryCreateDto
    {
        [Range(1, long.MaxValue, ErrorMessage = "La organización es obligatoria.")]
        public long OrganizationId { get; set; }

        // Lista de detalles para agregar artículos al inventario
        public List<InventoryDetailCreateDTO>? Details { get; set; }
    }
}
