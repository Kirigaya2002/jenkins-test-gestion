using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class CreateArticleDTO
    {
        public string? SourceInfo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "El porcentaje de impuesto es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de impuesto debe estar entre 0 y 100.")]
        public decimal TaxPercentage { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo.")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "El porcentaje de ganancia es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de ganancia debe estar entre 0 y 100.")]
        public decimal ProfitPercentage { get; set; }
    }
}




