using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class UpdateClientDTO
    {
        [Required(ErrorMessage = "La identificación del cliente es obligatoria.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "La identificación del cliente debe tener exactamente 9 dígitos.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La identificación del cliente debe contener solo dígitos numericos.")]
        public string Identification { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras.")]
        public string Lastname { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El numero teléfonico es obligatorio.")]
        public string? Phone { get; set; }

        public bool? Active { get; set; }
    }
}
