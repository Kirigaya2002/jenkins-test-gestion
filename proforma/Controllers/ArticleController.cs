using Microsoft.AspNetCore.Mvc;
using proforma.Models.DTO;
using proforma.Services;

namespace proforma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticlesController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpPost]
        public async Task<ActionResult<ArticleDTO>> CreateArticle([FromBody] CreateArticleDTO createArticleDto, [FromQuery] string barcode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdArticle = await _articleService.CreateArticleAsync(createArticleDto, barcode);
            return CreatedAtAction(nameof(GetArticleById), new { id = createdArticle.Id }, createdArticle);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleDTO>>> GetArticles()
        {
            var articles = await _articleService.GetArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleDTO>> GetArticleById(ulong id)
        {
            var articleDto = await _articleService.GetArticleByIdAsync(id);
            if (articleDto == null)
            {
                return NotFound($"Artículo con ID {id} no encontrado o eliminado.");
            }
            return Ok(articleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(ulong id, [FromBody] UpdateArticleDTO updateArticleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _articleService.UpdateArticleAsync(id, updateArticleDto);
            if (!success)
            {
                return NotFound("Artículo no encontrado o eliminado.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(ulong id)
        {
            var success = await _articleService.DeleteArticleAsync(id);
            if (!success)
            {
                return NotFound("Artículo no encontrado o ya eliminado.");
            }

            return NoContent();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreArticle(ulong id)
        {
            var success = await _articleService.RestoreArticleAsync(id);
            if (!success)
            {
                return NotFound("Artículo no encontrado o no está eliminado.");
            }

            return NoContent();
        }

        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<ArticleDTO>>> GetDeletedArticles()
        {
            var articles = await _articleService.GetDeletedArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ArticleDTO>>> SearchArticles([FromQuery] string term)
        {
            var results = await _articleService.SearchArticlesAsync(term);
            return Ok(results);
        }

    }
}
