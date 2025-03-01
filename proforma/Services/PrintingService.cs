using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services
{
    public class PrintingService
    {
        private readonly ProformaContext _context;

        public PrintingService(ProformaContext context)
        {
            _context = context;
        }

       
        public async Task<IEnumerable<PrintingTemplate>> GetPrintingTemplatesAsync()
        {
            return await _context.PrintingTemplates
                .Where(pt => pt.DeletedAt == null)
                .ToListAsync();
        }

        
        public async Task CreatePrintingTemplateAsync(PrintingTemplateDTO printingTemplateDTO)
        {
            if (printingTemplateDTO == null)
                throw new ArgumentNullException(nameof(printingTemplateDTO), "Los datos de la plantilla no pueden ser nulos.");

            if (string.IsNullOrWhiteSpace(printingTemplateDTO.TemplateName))
                throw new ArgumentException("El nombre de la plantilla no puede estar vacío.");

            if (printingTemplateDTO.Copies <= 0)
                throw new ArgumentException("El número de copias debe ser mayor a cero.");

            var printingTemplate = new PrintingTemplate
            {
               
                UserId = printingTemplateDTO.UserId,
                TemplateName = printingTemplateDTO.TemplateName,
                PageOrientation = printingTemplateDTO.PageOrientation,
                ColorMode = printingTemplateDTO.ColorMode,
                Copies = printingTemplateDTO.Copies,
                PaperSize = printingTemplateDTO.PaperSize,
                HeaderHtml = printingTemplateDTO.HeaderHtml,
                FooterHtml = printingTemplateDTO.FooterHtml,
                CreatedAt = DateTime.UtcNow
            };

            await _context.PrintingTemplates.AddAsync(printingTemplate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePrintingTemplateAsync(ulong id, PrintingTemplateDTO printingTemplateDTO)
        {
            if (printingTemplateDTO == null)
                throw new ArgumentNullException(nameof(printingTemplateDTO), "Los datos de la plantilla no pueden ser nulos.");

            var printingTemplate = await _context.PrintingTemplates.FindAsync(id);
            if (printingTemplate == null)
                throw new KeyNotFoundException("La plantilla de impresión no existe.");

            if (string.IsNullOrWhiteSpace(printingTemplateDTO.TemplateName))
                throw new ArgumentException("El nombre de la plantilla no puede estar vacío.");

            printingTemplate.UserId = printingTemplateDTO.UserId;
            printingTemplate.TemplateName = printingTemplateDTO.TemplateName;
            printingTemplate.PageOrientation = printingTemplateDTO.PageOrientation;
            printingTemplate.ColorMode = printingTemplateDTO.ColorMode;
            printingTemplate.Copies = printingTemplateDTO.Copies;
            printingTemplate.PaperSize = printingTemplateDTO.PaperSize;
            printingTemplate.HeaderHtml = printingTemplateDTO.HeaderHtml;
            printingTemplate.FooterHtml = printingTemplateDTO.FooterHtml;
            printingTemplate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<PrintingTemplate> GetPrintingTemplateByIdAsync(ulong id)
        {
            var template = await _context.PrintingTemplates.FindAsync(id);
            if (template == null)
                throw new KeyNotFoundException("La plantilla de impresión no existe.");

            return template;
        }

        public async Task<IEnumerable<PrintingTemplate>> GetPrintingTemplatesByUserIdAsync(ulong userId)
        {
            return await _context.PrintingTemplates
                .Where(pt => pt.UserId == userId && pt.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<PrintingTemplate>> GetDeletedPrintingTemplatesAsync()
        {
            return await _context.PrintingTemplates
                .Where(pt => pt.DeletedAt != null)
                .ToListAsync();
        }

        public async Task RestorePrintingTemplateAsync(ulong id)
        {
            var printingTemplate = await _context.PrintingTemplates.FindAsync(id);
            if (printingTemplate == null)
                throw new KeyNotFoundException("La plantilla de impresión no existe.");

            printingTemplate.DeletedAt = null;
            await _context.SaveChangesAsync();
        }
        public async Task DeletePrintingTemplateAsync(ulong id)
        {
            var printingTemplate = await _context.PrintingTemplates.FindAsync(id);
            if (printingTemplate == null || printingTemplate.DeletedAt != null)
                throw new KeyNotFoundException("La plantilla de impresión no existe o ya está eliminada.");

            printingTemplate.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
