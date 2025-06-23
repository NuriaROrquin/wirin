using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParagraphAnnotationController : ControllerBase
    {
        private readonly ParagraphAnnotationService _orderParagraphAnnotationService;

        public ParagraphAnnotationController(ParagraphAnnotationService orderParagraphAnnotationService)
        {
            _orderParagraphAnnotationService = orderParagraphAnnotationService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveParagraphAnnotationAsync(OrderParagrapAnnotationsRequest request)
        {
            try
            {
                await _orderParagraphAnnotationService.SaveParagraphAnnotationAsync(request);
                return Ok("Paragraph annotation processed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la anotación: {ex.Message}");
            }
        }

    }
}

