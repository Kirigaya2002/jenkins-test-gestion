using System.ComponentModel.DataAnnotations;

namespace proforma.Models.DTO
{
    public class CreateDTOProforma
    {
        //[Required(ErrorMessage = "Numero de factura es requerida.")]
        //[StringLength(50, ErrorMessage = "no puede ser mas de 50 caracteres.")]
        public string InvoiceNumber { get; set; } = null!;

        [Required(ErrorMessage = "Organization es requerida.")]
        public ulong OrganizationId { get; set; }

        [Required(ErrorMessage = "Cliente es requerido.")]
        public ulong ClientId { get; set; }
        public string? Comment { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Subtotal debe ser positivo.")]
        public decimal Subtotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Impuesto debe ser positivo.")]
        public decimal Taxes { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Descuento debe ser positivo.")]
        public decimal DiscountTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total debe ser positivo.")]
        public decimal Total { get; set; }

        //agregar proformas detail (articulos)
        public List<ProformaDetailDTO> Details { get; set; } = new();
    }
}
