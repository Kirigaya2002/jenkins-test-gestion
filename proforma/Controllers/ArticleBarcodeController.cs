using Microsoft.AspNetCore.Mvc;
using proforma.Models;
using proforma.Services;

namespace proforma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleBarcodeController: Controller
    {
        private readonly ArticleBarcodeService _articleBarcodeService;

        public ArticleBarcodeController(ArticleBarcodeService articleBarcodeService)
        {
            _articleBarcodeService = articleBarcodeService;
        }

        //obtener todos los barcode con Articulos
        [HttpGet]
        public async Task<ActionResult<List<ArticleBarcode>>> GetAllArticleBarcodes()
        {
            var barcodes = await _articleBarcodeService.GetAllArticleBarcodesAsync();
            return Ok(barcodes);
        }
    }
}
