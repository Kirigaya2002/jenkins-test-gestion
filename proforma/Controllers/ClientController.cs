using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ClientController : Controller
    {
        private readonly ClientService _clientService;

        public ClientController(ClientService clientService)
        {
            _clientService = clientService;
        }

        // POST: api/Client/{id} (Agregar clientes) (modificado)
        [HttpPost("{idOrg}")]
        public async Task<IActionResult> CreateClient(ulong idOrg, [FromBody] CreateClientDTO createClientDto)
        {
            //Verificar si el modelo no es válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                // Verificar si el cliente ya existe en base de datos
                if (await _clientService.DNIExistsAsync(createClientDto.Identification))
                {
                    // Si está registrado, se agrega a la organización actual
                    var person = await _clientService.GetClientByIdentificationAsync(createClientDto.Identification);
                    if (person != null)
                    {
                        await _clientService.AddClientToOrganizationAsync(person.Id, idOrg);
                        return Ok();
                    }
                }
                else // Si no está registrado, se crea un nuevo cliente
                {
                    // Validación de datos
                    if (await _clientService.EmailExistsAsync(createClientDto.Email))
                    {
                        return BadRequest("El correo electrónico ya está registrado.");
                    }

                    if (await _clientService.PhoneExistsAsync(createClientDto.Phone))
                    {
                        return BadRequest("El teléfono ya está registrado.");
                    }

                    if (await _clientService.DNIExistsAsync(createClientDto.Identification))
                    {
                        return BadRequest($"El cliente con la cédula {createClientDto.Identification} ya está registrado.");
                    }

                    // Crear el cliente
                    var client = await _clientService.CreateClientAsync(createClientDto, idOrg);
                    return Ok(client);
                }
            }
            return BadRequest("Error desconocido.");
        }

        // GET: api/Client/{id} (Obtener un clientes por ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetClinetById(ulong id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            return Ok(client);
        }

        // GET: api/Client/organization/{idOrg} (Obtener clientes habilitados por organización) (modificado)
        [HttpGet("organization/{idOrg}")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClientsByOrganization(ulong idOrg, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (clients, totalPages) = await _clientService.GetClientsByOrganizationAsync(idOrg, page, pageSize);

            return Ok(new
            {
                clients,
                totalPages,
                currentPage = page
            });
        }

        // GET: api/Client/deleted (Listar paginacion clientes inhabilitados) (modificado)
        [HttpGet("deleted/{idOrg}")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClientsEliminated(ulong idOrg, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (clients, totalPages) = await _clientService.GetClientsEliminatedByOrganizationAsync(idOrg, page, pageSize);

            return Ok(new
            {
                clients,
                totalPages,
                currentPage = page
            });
        }

        // GET: api/Client/search (Buscar clientes) (modificado)
        [HttpGet("search/{idOrg}")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> SearchClients([FromQuery] string name, ulong idOrg)
        {
            var clients = await _clientService.SearchClientsAsync(name, idOrg);
            return Ok(clients);
        }

        // PUT: api/Client/{id}/(Modificar clientes por ID)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClients(ulong id, [FromBody] UpdateClientDTO clientDto)
        {
            if (!await _clientService.UpdateClientAsync(id, clientDto))
            {
                return NotFound("Cliente no encontrada.");
            }
            return NoContent();
        }

        // DELETE: api/Client/{clientId}/organization/{organizationId} (Eliminar cliente de una organización específica) (modificado)
        [HttpDelete("{clientId}/organization/{organizationId}")]
        public async Task<IActionResult> DeleteClientOrganization(ulong clientId, ulong organizationId)
        {
            var success = await _clientService.DeleteClientOrganizationAsync(clientId, organizationId);
            if (!success)
            {
                return NotFound("El cliente no pertenece a esta organización.");
            }
            return NoContent();
        }

        // PUT: api/Client/{clientId}/restore/{orgId} (Revertir un cliente eliminado) (modificado)
        [HttpPut("{clientId}/restore/{orgId}")]
        public async Task<IActionResult> RestoreClient(ulong clientId, ulong orgId)
        {
            var success = await _clientService.RestoreClientOrganizationAsync(clientId, orgId);
            if (!success)
            {
                return NotFound("El cliente no esta eliminado");
            }
            return NoContent();
        }

        [HttpGet("by-identification/{identification}")]
        public async Task<ActionResult<ClientDTO>> GetClientByIdentification(string identification)
        {
            var client = await _clientService.GetClientByIdentificationAsync(identification);
            if (client == null)
            {
                return NotFound(new { message = "Client not found." });
            }
            return Ok(client);
        }

    }
}
