using Microsoft.AspNetCore.Mvc;
using proforma.Services;
using proforma.Models.DTO;

namespace proforma.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly OrganizationService _organizationService;

    public OrganizationsController(OrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    // POST: api/Organizations (Crear una organización)
    [HttpPost]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDTO createOrganizationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validación de datos (opcional)
        if (await _organizationService.NameExistsAsync(createOrganizationDto.Name))
        {
            return BadRequest("El nombre de la organización ya está registrado.");
        }

        if (await _organizationService.PhoneExistsAsync(createOrganizationDto.Phone))
        {
            return BadRequest($"El teléfono {createOrganizationDto.Phone} ya está registrado.");
        }

        if (await _organizationService.EmailExistsAsync(createOrganizationDto.Email))
        {
            return BadRequest($"El email {createOrganizationDto.Email} ya está registrado.");
        }

        // Crear la organización
        var organization = await _organizationService.CreateOrganizationAsync(createOrganizationDto);
        return CreatedAtAction(nameof(GetOrganizationById), new { id = organization.Id }, organization);
    }

    // GET: api/Organizations (Listar organizaciones)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> GetOrganizations()
    {
        var organizations = await _organizationService.GetOrganizationsAsync();
        return Ok(organizations);
    }

    // GET: api/Organizations/{id} (Obtener una organización por ID)
    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationDTO>> GetOrganizationById(ulong id)
    {
        var organization = await _organizationService.GetOrganizationByIdAsync(id);
        if (organization == null)
        {
            return NotFound("Organización no encontrada.");
        }

        return Ok(organization);
    }

    // GET: api/Organizations/{userId}/list (Listar las organizaciones de un usuario)
    [HttpGet("{userId}/list")]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> GetOrganizationsByUserId(ulong userId)
    {
        var organizations = await _organizationService.GetOrganizationsByUserIdAsync(userId);
        return Ok(organizations);
    }

    //DELETE: api/Organizations/{id} (Eliminar una organización por ID)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrganization(ulong id)
    {
        if (!await _organizationService.DeleteOrganizationAsync(id))
        {
            return NotFound("Organización no encontrada.");
        }
        return NoContent();
    }

    // PUT: api/Organizations/{id}/restore (Revertir una organización eliminada)
    [HttpPut("{id}/restore")]
    public async Task<IActionResult> RestoreOrganization(ulong id)
    {
        if (!await _organizationService.RestoreOrganizationAsync(id))
        {
            return NotFound("Organización no encontrada o ya restaurada.");
        }
        return NoContent();
    }

    // GET: api/Organizations/{orgId}/deleted (Listar organizaciones eliminadas de un usuario)
    [HttpGet("{userId}/deleted")]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> GetDeletedOrganizations(ulong userId)
    {
        var organizations = await _organizationService.GetDeletedOrganizationsAsync(userId);
        return Ok(organizations);
    }

    // PUT: api/Organizations/{id} (Actualizar una organización por ID)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganization(ulong id, [FromBody] UpdateOrganizationDTO updateOrganizationDto)
    {
        var organization = await _organizationService.GetOrganizationByIdAsync(id);
        //if donde pregunto si el nombre es el mismo que tenía antes de actualizar
        if (!organization.Name.Equals(updateOrganizationDto.Name)){

            if (await _organizationService.NameExistsAsync(updateOrganizationDto.Name))
            {
                return BadRequest("El nombre de la organización ya está registrado.");
            }
        }

        await _organizationService.UpdateOrganizationAsync(id, updateOrganizationDto);

        return NoContent();
    }

    // GET: api/Organizations/search (Buscar organizaciones por nombre)
    [HttpGet("search/{userId}")]
    public async Task<ActionResult<IEnumerable<OrganizationDTO>>> SearchOrganizations(ulong userId, [FromQuery] string text)
    {
        var organizations = await _organizationService.SearchOrganizationsAsync(text, userId);
        return Ok(organizations);
    }
}
