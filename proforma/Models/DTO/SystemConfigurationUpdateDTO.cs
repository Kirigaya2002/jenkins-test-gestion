using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class SystemConfigurationUpdateDTO
    {
        [Required(ErrorMessage = "El valor es obligatorio.")]
        [RegularExpression("^(dark|light)$", ErrorMessage = "El valor solo puede ser 'dark' o 'light'.")]
        public string Value { get; set; } = null!;
    }
}
