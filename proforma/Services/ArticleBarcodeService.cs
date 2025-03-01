using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services
{
    public class ArticleBarcodeService 
    {
        private readonly ProformaContext _context;

        public ArticleBarcodeService(ProformaContext context)
        {
            _context = context;
        }

        //obtener codigos de barra con Articulos
        public async Task<List<ArticleBarcode>> GetAllArticleBarcodesAsync()
        {
            return await _context.ArticleBarcodes
            .Include(ab => ab.Article)
            .Select(ab => new ArticleBarcode
            {
                Id = ab.Id,
                ArticleId = ab.ArticleId,
                Barcode = ab.Barcode,
                Article = new Article
                {
                    Id = ab.Article.Id,
                    SourceInfo = ab.Article.SourceInfo,
                    Description = ab.Article.Description,
                    TaxPercentage = ab.Article.TaxPercentage,
                    Cost = ab.Article.Cost,
                    NetProfit = ab.Article.NetProfit,
                    Total = ab.Article.Total,
                    TaxNet = ab.Article.TaxNet,
                    Subtotal = ab.Article.Subtotal,
                    TaxPercentual = ab.Article.TaxPercentual,
                }
            })
            .ToListAsync();
        }
    }
}
