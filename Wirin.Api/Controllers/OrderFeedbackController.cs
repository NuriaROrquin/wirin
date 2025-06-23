using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderFeedbackController : ControllerBase
{
    private readonly OrderFeedbackService _orderFeedbackService;

    public OrderFeedbackController(OrderFeedbackService orderFeedbackService)
    {
        _orderFeedbackService = orderFeedbackService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderFeedbackResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrderFeedbacks()
    {
        var orderFeedbacks = await _orderFeedbackService.GetAllOrderFeedbacksAsync();
        return Ok(OrderFeedbackResponse.ListFromDomain(orderFeedbacks));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderFeedbackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderFeedbackById(int id)
    {
        var orderFeedback = await _orderFeedbackService.GetOrderFeedbackByIdAsync(id);
        if (orderFeedback == null)
        {
            return NotFound();
        }
        return Ok(OrderFeedbackResponse.FromDomain(orderFeedback));
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderFeedbackResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrderFeedback([FromBody] CreateOrderFeedbackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var orderFeedback = new OrderFeedback
        {
            StudentId = request.StudentId,
            FeedbackText = request.FeedbackText,
            Stars = request.Stars,
            OrderId = request.OrderId
        };

        try
        {
            var created = await _orderFeedbackService.AddOrderFeedbackAsync(orderFeedback);
            return CreatedAtAction(nameof(GetOrderFeedbackById), new { id = created.Id }, OrderFeedbackResponse.FromDomain(created));

        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderFeedback(int id, [FromBody] UpdateOrderFeedbackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var orderFeedbackToUpdate = new OrderFeedback
        {
            Id = id,
            StudentId = request.StudentId,
            FeedbackText = request.FeedbackText,
            Stars = request.Stars,
            OrderId = request.OrderId
        };

        try
        {
            var updated = await _orderFeedbackService.UpdateOrderFeedbackAsync(id, orderFeedbackToUpdate);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent(); // ✅ Esto es lo que espera el test
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrderFeedback(int id)
    {
        var deleted = await _orderFeedbackService.DeleteOrderFeedbackAndHandleNotFoundAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}