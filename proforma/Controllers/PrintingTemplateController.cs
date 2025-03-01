using Microsoft.AspNetCore.Mvc;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrintingTemplateController : ControllerBase
    {
        private readonly PrintingService _printingService;

        public PrintingTemplateController(PrintingService printingService)
        {
            _printingService = printingService;
        }

        // api/PrintingTemplate
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrintingTemplate>>> GetPrintingTemplates()
        {
            var templates = await _printingService.GetPrintingTemplatesAsync();
            return Ok(templates);
        }

        // api/PrintingTemplate/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PrintingTemplate>> GetPrintingTemplate(ulong id)
        {
            var template = await _printingService.GetPrintingTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound($"Plantilla con ID {id} no encontrada o eliminada.");
            }
            return Ok(template);
        }

        // api/PrintingTemplate/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PrintingTemplate>>> GetPrintingTemplatesByUser(ulong userId)
        {
            var templates = await _printingService.GetPrintingTemplatesByUserIdAsync(userId);
            return Ok(templates);
        }

        // api/PrintingTemplate/deleted
        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<PrintingTemplate>>> GetDeletedPrintingTemplates()
        {
            var templates = await _printingService.GetDeletedPrintingTemplatesAsync();
            return Ok(templates);
        }

        // api/PrintingTemplate
        [HttpPost]
        public async Task<ActionResult> CreatePrintingTemplate([FromBody] PrintingTemplateDTO printingTemplateDTO)
        {
            if (printingTemplateDTO == null)
            {
                return BadRequest("Los datos de la plantilla son obligatorios.");
            }
            if (string.IsNullOrWhiteSpace(printingTemplateDTO.TemplateName))
            {
                return BadRequest("El nombre de la plantilla no puede estar vacío.");
            }
            if (printingTemplateDTO.Copies.HasValue && printingTemplateDTO.Copies <= 0)
            {
                return BadRequest("El número de copias debe ser mayor a cero.");
            }

            try
            {
                await _printingService.CreatePrintingTemplateAsync(printingTemplateDTO);
                return CreatedAtAction(nameof(GetPrintingTemplate), new { id = printingTemplateDTO.UserId }, printingTemplateDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // api/PrintingTemplate/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePrintingTemplate(ulong id, [FromBody] PrintingTemplateDTO printingTemplateDTO)
        {
            if (printingTemplateDTO == null)
            {
                return BadRequest("Los datos de la plantilla son obligatorios.");
            }
            if (string.IsNullOrWhiteSpace(printingTemplateDTO.TemplateName))
            {
                return BadRequest("El nombre de la plantilla no puede estar vacío.");
            }
            if (printingTemplateDTO.Copies.HasValue && printingTemplateDTO.Copies <= 0)
            {
                return BadRequest("El número de copias debe ser mayor a cero.");
            }

            try
            {
                await _printingService.UpdatePrintingTemplateAsync(id, printingTemplateDTO);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Plantilla con ID {id} no encontrada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // api/PrintingTemplate/restore/{id}
        [HttpPut("restore/{id}")]
        public async Task<ActionResult> RestoreDeletedPrintingTemplate(ulong id)
        {
            try
            {
                await _printingService.RestorePrintingTemplateAsync(id);
                return Ok("Plantilla restaurada exitosamente.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Plantilla con ID {id} no encontrada o no está eliminada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // api/PrintingTemplate/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePrintingTemplate(ulong id)
        {
            try
            {
                await _printingService.DeletePrintingTemplateAsync(id);
                return Ok("Plantilla eliminada exitosamente.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Plantilla con ID {id} no encontrada o ya está eliminada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
