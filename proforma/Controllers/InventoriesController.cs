using Microsoft.AspNetCore.Mvc;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class InventoriesController : ControllerBase
{
    private readonly InventoryService _service;

    public InventoriesController(InventoryService service)
    {
        _service = service;
    }

    // POST: api/Inventories
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InventoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Inventories/{id}
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] InventoryUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PATCH (Soft Delete): api/Inventories/{id}/soft-delete
    [HttpPatch("{id:long}/soft-delete")]
    public async Task<IActionResult> SoftDelete(long id)
    {
        try
        {
            var result = await _service.SoftDeleteAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE (Hard Delete): api/Inventories/{id}
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> HardDelete(long id)
    {
        try
        {
            await _service.HardDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET: api/Inventories/{orgId}/details
    [HttpGet("{orgId}/details")]
    public async Task<IActionResult> GetDetails(long orgId)
    {
        try
        {
            var result = await _service.GetInventoryDetailsByOrganizationId(orgId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST : api/Inventories/{orgId}/add-article
    [HttpPost("{orgId}/add-article")]
    public async Task<IActionResult> AddArticle(long orgId, [FromQuery] string articleCode, [FromQuery] int quantity)
    {
        if (string.IsNullOrEmpty(articleCode))
            return BadRequest("El código del artículo es obligatorio.");

        if (quantity <= 0)
            return BadRequest("La cantidad debe ser mayor a cero.");

        try
        {
            await _service.AddArticleToInventoryAsync(orgId, articleCode, quantity);
            return Ok(new { message = "Artículo agregado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}
