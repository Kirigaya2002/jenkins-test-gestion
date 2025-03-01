using Microsoft.EntityFrameworkCore;
using proforma.Models;
using proforma.Models.DTO;

namespace proforma.Services
{
    public class ArticleService
    {
        private readonly ProformaContext _context;

        public ArticleService(ProformaContext context)
        {
            _context = context;
        }

        // Crear Artículo
        public async Task<ArticleDTO> CreateArticleAsync(CreateArticleDTO createArticleDto, string barcode)
        {
            var article = new Article
            {
                SourceInfo = createArticleDto.SourceInfo,
                Description = createArticleDto.Description,
                TaxPercentage = createArticleDto.TaxPercentage,
                Cost = createArticleDto.Cost,
                ProfitPercentage = createArticleDto.ProfitPercentage,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            CalculateFinancials(article);

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            //agregamos a la tabla relacional article_barcode
            var articleBarcode = new ArticleBarcode
            {
                ArticleId = article.Id,
                Barcode = barcode
            };
            _context.ArticleBarcodes.Add(articleBarcode);
            await _context.SaveChangesAsync();

            return MapToArticleDTO(article);
        }

        // Obtener por ID
        public async Task<ArticleDTO?> GetArticleByIdAsync(ulong id)
        {
            var article = await _context.Articles
                .Where(a => a.Id == id && a.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return null;
            }

            return MapToArticleDTO(article);
        }

        // Obtener Todos los Artículos (no eliminados)
        public async Task<IEnumerable<ArticleDTO>> GetArticlesAsync()
        {
            var articles = await _context.Articles
                .Where(a => a.DeletedAt == null)
                .ToListAsync();

            return articles.Select(a => MapToArticleDTO(a));
        }

        // Actualizar 
        public async Task<bool> UpdateArticleAsync(ulong id, UpdateArticleDTO updateArticleDto)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);
            if (article == null)
            {
                return false;
            }

            article.SourceInfo = updateArticleDto.SourceInfo ?? article.SourceInfo;
            article.Description = updateArticleDto.Description;
            article.TaxPercentage = updateArticleDto.TaxPercentage;
            article.Cost = updateArticleDto.Cost;
            article.ProfitPercentage = updateArticleDto.ProfitPercentage;

            if (updateArticleDto.Active.HasValue)
            {
                article.Active = updateArticleDto.Active.Value;
            }
            CalculateFinancials(article);

            article.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        // Eliminar (Soft Delete)
        public async Task<bool> DeleteArticleAsync(ulong id)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);
            if (article == null)
            {
                return false;
            }

            article.DeletedAt = DateTime.UtcNow;
            article.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        // Restaurar 
        public async Task<bool> RestoreArticleAsync(ulong id)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt != null);
            if (article == null)
            {
                return false;
            }

            article.DeletedAt = null;
            article.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        // Listar Eliminados
        public async Task<IEnumerable<ArticleDTO>> GetDeletedArticlesAsync()
        {
            var articles = await _context.Articles
                .Where(a => a.DeletedAt != null)
                .ToListAsync();

            return articles.Select(a => MapToArticleDTO(a));
        }

        //buscar artículos por todos sus campos
        public async Task<IEnumerable<ArticleDTO>> SearchArticlesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetArticlesAsync();
            }

            searchTerm = searchTerm.ToLower();

            var articles = await _context.Articles
                .Where(a => a.DeletedAt == null &&
                           (
                               (a.SourceInfo != null && EF.Functions.Like(a.SourceInfo.ToLower(), $"%{searchTerm}%")) ||
                               EF.Functions.Like(a.Description.ToLower(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.TaxPercentage.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.Cost.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.ProfitPercentage.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.NetProfit.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.Subtotal.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.TaxNet.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.TaxPercentual.ToString(), $"%{searchTerm}%") ||
                               EF.Functions.Like(a.Total.ToString(), $"%{searchTerm}%")
                           ))
                .ToListAsync();

            return articles.Select(a => MapToArticleDTO(a));
        }

        //Obtener un articulo por su codigo en la tabla relacional article_barcode
        public async Task<ArticleDTO?> GetArticleByCodeAsync(string code)
        {
            var article = await _context.ArticleBarcodes
                .Where(ab => ab.Barcode == code)
                .Select(ab => ab.Article)
                .FirstOrDefaultAsync();
            if (article == null)
            {
                return null;
            }
            return MapToArticleDTO(article);
        }

        // AUX
        private void CalculateFinancials(Article article)
        {
            article.Subtotal = article.Cost + (article.Cost * article.ProfitPercentage / 100);
            article.NetProfit = article.Subtotal * article.ProfitPercentage / 100;
            article.TaxNet = article.Subtotal * article.TaxPercentage / 100;
            article.TaxPercentual = article.TaxPercentage;
            article.Total = article.Subtotal + article.TaxNet;
        }

        // AUX
        private ArticleDTO MapToArticleDTO(Article article)
        {
            return new ArticleDTO
            {
                Id = article.Id,
                SourceInfo = article.SourceInfo,
                Description = article.Description,
                TaxPercentage = article.TaxPercentage,
                Cost = article.Cost,
                ProfitPercentage = article.ProfitPercentage,
                NetProfit = article.NetProfit,
                Subtotal = article.Subtotal,
                TaxNet = article.TaxNet,
                TaxPercentual = article.TaxPercentual,
                Total = article.Total,
                Active = article.Active,
                CreatedAt = article.CreatedAt
            };
        }
    }
}
