using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class CreateClientDTO
    {
        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "La identificación debe tener exactamente 9 dígitos.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La identificación debe contener solo dígitos numéricos.")]
        public string Identification { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no debe exceder los 50 caracteres.")]
        public string Lastname { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
        [StringLength(20, ErrorMessage = "El número de teléfono no debe exceder los 20 caracteres.")]
        public string Phone { get; set; } = null!;

        public bool? Active { get; set; }
    }
}