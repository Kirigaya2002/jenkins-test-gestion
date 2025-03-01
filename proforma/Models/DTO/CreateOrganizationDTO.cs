using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class CreateOrganizationDTO
    {
        [Required(ErrorMessage = "El nombre de la organización es obligatoria.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "La identificación debe tener exactamente 9 dígitos.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La identificación debe contener solo dígitos numéricos.")]
        public string? ManagerIdentification { get; set; }

        [Required(ErrorMessage = "El teléfono de la organización es obligatoria.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La dirección de la organización es obligatorio.")]
        public string? Address { get; set; }

        //(nuevo)
        [Required(ErrorMessage = "El user_id es obligatorio.")]
        public ulong UserId { get; set; }
    }
}
