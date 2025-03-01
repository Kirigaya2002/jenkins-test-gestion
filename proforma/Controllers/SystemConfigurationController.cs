using Microsoft.AspNetCore.Mvc;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemConfigurationController : ControllerBase
    {
        private readonly SystemConfigurationService _configurationService;

        public SystemConfigurationController(SystemConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetConfigurationById(long id)
        {
            var configuration = await _configurationService.GetConfigurationByIdAsync((ulong)id);
            if (configuration == null)
            {
                return NotFound($"Configuración con ID {id} no encontrada.");
            }
            return Ok(configuration);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateConfiguration(long id, [FromBody] SystemConfigurationUpdateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Los datos de la configuración son obligatorios.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // el valor debe ser "dark" o "light"
            if (!string.Equals(dto.Value, "dark", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(dto.Value, "light", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El valor de la configuración debe ser 'dark' o 'light'.");
            }

            try
            {
                var updatedConfiguration = await _configurationService.UpdateConfigurationAsync((ulong)id, dto);
                if (updatedConfiguration == null)
                {
                    return NotFound($"Configuración con ID {id} no encontrada.");
                }
                return Ok(updatedConfiguration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET: /api/SystemConfiguration/{userId}/theme
        [HttpGet("{userId}/theme")]
        public async Task<IActionResult> GetUserConfiguration(long userId)
        {
            var configurations = await _configurationService.GetConfigurationsAsync((ulong)userId);
            var themes = configurations.Where(c => c.Key == "theme").ToList();
            if (themes == null || !themes.Any())
            {
                return NotFound("Configuración de tema no encontrada.");
            }
            return Ok(themes);
        }
    }
}
