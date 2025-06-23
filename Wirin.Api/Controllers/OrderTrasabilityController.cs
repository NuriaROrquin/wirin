using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTrasabilityController : ControllerBase
    {

        private readonly OrderTrasabilityService _orderTrazabilityService;
        public OrderTrasabilityController(OrderTrasabilityService orderTrazabilityService)
        {
            _orderTrazabilityService = orderTrazabilityService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrderTrasabilityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrazabilitys()
        {

            var trazabilitys = await _orderTrazabilityService.GetAllOrderTrasabilities();
            if (trazabilitys == null)
            {
                return NotFound(new { message = "Sin Datos de Trazabilidad" });
            }
            return Ok(OrderTrasabilityResponse.ListFromDomain(trazabilitys));
        }

        [HttpGet("byOrderId/{orderId}")]
        [ProducesResponseType(typeof(List<OrderTrasabilityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderTrasabilitiesByOrderIdAsync(int orderId)
        {
            var trazabilitys = await _orderTrazabilityService.GetOrderTrasabilitiesByOrderIdAsync(orderId);
            if (trazabilitys == null )
            {
                return NotFound(new { message = "Sin Datos de Trazabilidad para este pedido" });
            }
            return Ok(OrderTrasabilityResponse.ListFromDomain(trazabilitys));

        }

        [HttpGet("byAction/{action}")]
        [ProducesResponseType(typeof(List<OrderTrasabilityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderTrasabilitiesByActionAsync(string action)
        {
            var trazabilitys = await _orderTrazabilityService.GetOrderTrasabilitiesByActionAsync(action);
            if (trazabilitys == null)
            {
                return NotFound(new { message = "Sin Datos de Trazabilidad para esta acción" });
            }
            return Ok(OrderTrasabilityResponse.ListFromDomain(trazabilitys));

        }

        [HttpGet("byUser/{userId}")]
        [ProducesResponseType(typeof(List<OrderTrasabilityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderTrasabilitiesByUserAsync(string userId)
        {
            var trazabilitys = await _orderTrazabilityService.GetOrderTrasabilitiesByUserAsync(userId);
            if (trazabilitys == null )
            {
                return NotFound(new { message = "Sin Datos de Trazabilidad para este usuario" });
            }


            return Ok(OrderTrasabilityResponse.ListFromDomain(trazabilitys));
        }

    }
}
