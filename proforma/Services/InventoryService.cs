using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using proforma.Services;

namespace proforma.Services
{
    public class InventoryService
    {
        private readonly ProformaContext _context;
        private readonly ArticleService _articleService;

        public InventoryService(ProformaContext context, ArticleService articleService)
        {
            _context = context;
            _articleService = articleService;
        }

        public async Task<Inventory> CreateAsync(InventoryCreateDto dto)
        {
            var inventory = new Inventory
            {
                OrganizationId = (ulong)dto.OrganizationId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            if (dto.Details != null && dto.Details.Any())
            {
                foreach (var detailDto in dto.Details)
                {
                    var detail = new InventoryDetail
                    {
                        InventoryId = inventory.Id,
                        ArticleId = (ulong)detailDto.ArticleId,
                        Quantity = detailDto.Quantity,
                        Notes = detailDto.Notes,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.InventoryDetail.Add(detail);
                }
                await _context.SaveChangesAsync();
            }

            return inventory;
        }

        public async Task<Inventory> UpdateAsync(long id, InventoryUpdateDTO dto)
        {
            var inventory = await _context.Inventories
                .Include(i => i.InventoryDetails)
                .FirstOrDefaultAsync(i => i.Id == (ulong)id && i.DeletedAt == null);
            if (inventory == null)
            {
                throw new Exception($"Inventory with ID {id} not found.");
            }

            inventory.OrganizationId = (ulong)dto.OrganizationId;
            inventory.UpdatedAt = DateTime.UtcNow;

            if (dto.Details != null)
            {
                _context.InventoryDetail.RemoveRange(inventory.InventoryDetails);
                foreach (var detailDto in dto.Details)
                {
                    var detail = new InventoryDetail
                    {
                        InventoryId = inventory.Id,
                        ArticleId = (ulong)detailDto.ArticleId,
                        Quantity = detailDto.Quantity,
                        Notes = detailDto.Notes,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.InventoryDetail.Add(detail);
                }
            }

            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory> SoftDeleteAsync(long id)
        {
            var inventory = await _context.Inventories.FindAsync((ulong)id);
            if (inventory == null)
                throw new Exception($"Inventory with ID {id} not found.");

            if (inventory.DeletedAt == null)
                inventory.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task HardDeleteAsync(long id)
        {
            var inventory = await _context.Inventories.FindAsync((ulong)id);
            if (inventory == null)
                throw new Exception($"Inventory with ID {id} not found.");

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
        }

        //MEtodo para obtener el inventario por id de organizacion
        public async Task<List<Inventory>> GetInventoryDetailsByOrganizationId(long organizationId)
        {
            var inventories = await _context.Inventories
                .Include(i => i.InventoryDetails)
                    .ThenInclude(id => id.Article) 
                .Where(i => i.OrganizationId == (ulong)organizationId && i.DeletedAt == null)
                .ToListAsync();
            return inventories;
        }

        //Metodo para actualizar la cantidad de un articulo vendido en el detalle de inventario de una organizacion
        public async Task UpdateStockAsync(string articleCode, int sold_quantity, long organizationId)
        {
            var article = await _articleService.GetArticleByCodeAsync(articleCode);
            if (article == null)
            {
                throw new Exception($"No se ha encontrado el artículo con el código {articleCode}");
            }

            var inventory = await _context.Inventories
                .Include(i => i.InventoryDetails)
                .FirstOrDefaultAsync(i => i.OrganizationId == (ulong)organizationId && i.DeletedAt == null);
            if (inventory == null)
            {
                throw new Exception($"No se ha encontrado un inventario para esta organización");
            }

            var detail = inventory.InventoryDetails.FirstOrDefault(d => d.ArticleId == article.Id);
            if (detail == null)
            {
                throw new Exception($"No se ha encontrado el artículo con el código {articleCode} en el inventario de esta organización");
            }

            detail.Quantity -= sold_quantity;
            detail.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        //Metodo para agregar un articulo al inventario de una organizacion
        public async Task AddArticleToInventoryAsync(long organizationId, string articleCode, int quantity)
        {
            var article = await _articleService.GetArticleByCodeAsync(articleCode);
            if (article == null)
            {
                throw new Exception($"No se ha encontrado el artículo con el código {articleCode}");
            }

            var inventory = await _context.Inventories
                .Include(i => i.InventoryDetails)
                .FirstOrDefaultAsync(i => i.OrganizationId == (ulong)organizationId && i.DeletedAt == null);
            if (inventory == null)
            {
                throw new Exception($"No se ha encontrado un inventario para esta organización");
            }

            var detail = inventory.InventoryDetails.FirstOrDefault(d => d.ArticleId == article.Id);
            if (detail == null)
            {
                detail = new InventoryDetail
                {
                    InventoryId = inventory.Id,
                    ArticleId = article.Id,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryDetail.Add(detail);
            }
            else
            {
                detail.Quantity += quantity;
                detail.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}
