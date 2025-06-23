using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Services;
using Wirin.Infrastructure.Dtos.Requests;
using Wirin.Infrastructure.Services;

namespace Wirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderParagraphController : ControllerBase
    {
        private readonly OrderParagraphService _orderParagraphService;
        private readonly UserService _userService;


        public OrderParagraphController(OrderParagraphService orderParagraphService, UserService userService)
        {
            _orderParagraphService = orderParagraphService;
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessOrdersAsync(SaveParagraphRequest request)
        {
            var trasabilityUserId = _userService.GetUserTrasabilityId(User);

            if (request == null)
            {
                return BadRequest("No paragraph to process.");
            }

           await _orderParagraphService.UpdateParagraphAsync(request, trasabilityUserId, trasabilityUserId);

            return Ok("Orders processed successfully.");
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(List<OrderParagraphResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetParagraphsByOrderIdAsync(int orderId)
        {
            var paragraphs = await _orderParagraphService.GetParagraphsByOrderIdAsync(orderId);
            if (paragraphs == null || !paragraphs.Any())
            {
                return NotFound(new { message = "No paragraphs found for this order." });
            }
            return Ok(paragraphs);
        }

    }
}
