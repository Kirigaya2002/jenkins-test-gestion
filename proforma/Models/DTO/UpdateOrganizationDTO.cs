using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class UpdateOrganizationDTO
    {
        [Required(ErrorMessage = "El nombre de la organización es obligatoria.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El numero teléfonico es obligatorio.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "La identificación del manager es obligatoria.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "La identificación del manager debe tener exactamente 9 dígitos.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "La identificación del manager debe contener solo dígitos numericos.")]
        public string ManagerIdentification { get; set; } = null!;

        //(nuevo)
        [Required(ErrorMessage = "El user_id es obligatorio.")]
        public ulong UserId { get; set; }
    }
}
