
using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Services
{
    public class ProformaService
    {
        private readonly ProformaContext _context;
        private readonly InventoryService _inventoryService;

        public ProformaService(ProformaContext context, InventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        //Listar Proformas
        public async Task<IEnumerable<ProformaDTO>> GetProformasAsync()
        {
            return await _context.Proformas
                .Where(p => p.DeletedAt == null)
                .Select(p => new ProformaDTO
                {
                    InvoiceNumber = p.InvoiceNumber,
                    OrganizationId = p.OrganizationId,
                    ClientId = p.ClientId,
                    Comment = p.Comment,
                    Subtotal = p.Subtotal,
                    Taxes = p.Taxes,
                    DiscountTotal = p.DiscountTotal,
                    Total = p.Total,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    DeletedAt = p.DeletedAt
                }).ToListAsync();
        }

        //Listar los detalles de una proforma
        public async Task<IEnumerable<ProformaDetailDTO>> GetProformaDetailsAsync(ulong proformaId)
        {
            return await _context.ProformaDetails
                .Where(pd => pd.ProformaId == proformaId)
                .Select(pd => new ProformaDetailDTO
                {
                    ProformaId = pd.ProformaId,
                    ArticleCode = pd.ArticleCode,
                    Quantity = pd.Quantity,
                    Discount = pd.Discount,
                    ItemComment = pd.ItemComment,
                    UnitPrice = pd.UnitPrice,
                    UnitTax = pd.UnitTax,
                    LineTotal = pd.LineTotal
                }).ToListAsync();
        }

        //Agregar una nueva proforma y agregar articulos a una proforma
        public async Task<Proforma> CreateProformaAsync(CreateDTOProforma createProformaDTO)
        {
            var proforma = new Proforma
            {
                InvoiceNumber = await GetNextInvoiceNumberAsync(createProformaDTO.OrganizationId),
                OrganizationId = createProformaDTO.OrganizationId,
                ClientId = createProformaDTO.ClientId,
                Comment = createProformaDTO.Comment,
                Subtotal = createProformaDTO.Subtotal,
                Taxes = createProformaDTO.Taxes,
                DiscountTotal = createProformaDTO.DiscountTotal,
                Total = createProformaDTO.Total,
                CreatedAt = DateTime.UtcNow
            };

            _context.Proformas.Add(proforma);
            await _context.SaveChangesAsync();

            // Agregar los detalles de la proforma
            foreach (var detail in createProformaDTO.Details)
            {
                var proformaDetail = new ProformaDetail
                {
                    ProformaId = proforma.Id,
                    ArticleCode = detail.ArticleCode,
                    Quantity = detail.Quantity,
                    Discount = detail.Discount,
                    ItemComment = detail.ItemComment,
                    UnitPrice = detail.UnitPrice,
                    UnitTax = detail.UnitTax,
                    LineTotal = detail.LineTotal,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ProformaDetails.Add(proformaDetail);
                await _context.SaveChangesAsync();

                // Restar el stock de los articulos vendidos en el inventario
                await _inventoryService.UpdateStockAsync(detail.ArticleCode, detail.Quantity, (long)proforma.OrganizationId);
            }

            return proforma;
        }

        //Obtener lista de profromas de una organizacion, usando el modelo completo de proforma, por lo que lleva el cliente, los detalles de la proforma y la organizacion
        public async Task<(IEnumerable<Proforma?> Proformas, int TotalCount)> GetProformasFromOrganizationAsync(ulong orgId, int page, int pageSize)
        {
            var query = _context.Proformas
                .Include(p => p.Client)
                .Include(p => p.ProformaDetails)
                .Include(p => p.Organization)
                .Where(p => p.OrganizationId == orgId && p.DeletedAt == null);

            int totalCount = await query.CountAsync(); 

            var proformas = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (proformas, totalCount);
        }

        //Asignacion de codigos
        private async Task<string> GetNextInvoiceNumberAsync(ulong organizationId)
        {
            var lastProforma = await _context.Proformas
                .Where(p => p.OrganizationId == organizationId)
                .OrderByDescending(p => p.InvoiceNumber)
                .FirstOrDefaultAsync();

            if (lastProforma != null)
            {
                // Extraer la parte numérica del InvoiceNumber
                var match = System.Text.RegularExpressions.Regex.Match(lastProforma.InvoiceNumber, @"\d+");
                if (match.Success && long.TryParse(match.Value, out long lastCode))
                {
                    return (lastCode + 1)+"";
                }
            }

            // Si no hay registros previos, empezar con "INV-1001"
            return "1001";
        }

    }
}
