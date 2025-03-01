using Microsoft.AspNetCore.Mvc;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProformaController : ControllerBase
    {
        private readonly ProformaService _proformaService;

        public ProformaController(ProformaService proformaService)
        {
            _proformaService = proformaService;
        }

        //GET: api/Proforma
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProformaDTO>>> GetProformasAsync()
        {
            var proforma = await _proformaService.GetProformasAsync();
            return Ok(proforma);
        }

        //GET: api/Proforma/details/{proformaId}
        [HttpGet("details/{proformaId}")]
        public async Task<ActionResult<IEnumerable<ProformaDetailDTO>>> GetProformaDetailsAsync(ulong proformaId)
        {
            var proformaDetails = await _proformaService.GetProformaDetailsAsync(proformaId);
            return Ok(proformaDetails);
        }

        //GET: api/Proforma/{orgId}
        [HttpGet("{orgId}")]
        public async Task<ActionResult<IEnumerable<Proforma>>> GetProformasFromOrganizationAsync(ulong orgId, [FromQuery] int page = 1, [FromQuery] int pageSize = 2)
        {

            var (proformas, totalCount) = await _proformaService.GetProformasFromOrganizationAsync(orgId, page, pageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return Ok(new
            {
                proforma = proformas,  
                totalPages = totalPages,
                currentPage = page 
            });

        }

        // POST: api/Proforma
        [HttpPost]
        public async Task<ActionResult<ProformaDTO>> CreateProformaAsync([FromBody] CreateDTOProforma createProformaDTO)
        {
            
            if (createProformaDTO == null)
            {
                return BadRequest("Infromacion de la proforma es necesaria");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            await _proformaService.CreateProformaAsync(createProformaDTO);

            return Ok();
        }
    }
}
