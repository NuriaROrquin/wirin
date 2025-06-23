using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderDeliveryController : ControllerBase
{
    private OrderDeliveryService _orderDeliveryService;
    private UserService _userService;

    public OrderDeliveryController(OrderDeliveryService orderDeliveryService, UserService userService)
    {
        _orderDeliveryService = orderDeliveryService;
        _userService = userService;
    }

    // GET: api/OrderDeliveryEntities
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDelivery>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDelivery>>> GetOrderDeliveries()
    {
        return await _orderDeliveryService.GetAll();
    }

    [HttpGet("WithOrders")]
    [ProducesResponseType(typeof(List<OrderDelivery>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderDelivery>>> GetOrderDeliveriesWithOrders()
    {
        try
        {
            var result = await _orderDeliveryService.GetAllWithOrders();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }

    }

    //GetAllWithOrdersByStudentId
    [HttpGet("WithOrders/{studentId}")]
    [ProducesResponseType(typeof(List<OrderDelivery>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderDelivery>>> GetOrderDeliveriesWithOrdersByStudentId(string studentId)
    {
        try
        {
            var result = await _orderDeliveryService.GetAllWithOrdersByStudentId(studentId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }


    // GET: api/OrderDeliveryEntities/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDelivery), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderDeliveryEntity(int id)
    {
       return (IActionResult) await _orderDeliveryService.GetById(id);
    }

    // PUT: api/OrderDeliveryEntities/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutOrderDeliveryEntity(int id, OrderDelivery od)
    {
        if (id != od.Id)
        {
            return BadRequest();
        }

        try
        {
            await _orderDeliveryService.UpdateAsync(id, od);
        }
        catch (DbUpdateConcurrencyException)
        {
         throw;
        }

        return NoContent();
    }

    // POST: api/OrderDeliveryEntities
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task PostOrderDeliveryEntity(CreateOrderDeliveryRequest req)
    {
        var user = _userService.GetUserTrasabilityId(User);

        OrderDelivery od = new OrderDelivery
        {
            Title = req.Title,
            Status = req.Status,
            StudentId = req.StudentId,
            UserId = user
        };

        await _orderDeliveryService.Create(od);
    }

    [HttpPost("performDelivery")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostPerformDelivery([FromBody] PerformDeliveryRequest request)
    {
        var trasabilityUserId = _userService.GetUserTrasabilityId(User);
        try
        {

            await _orderDeliveryService.PerformDeliveryAsync(request, trasabilityUserId);

            return Ok(new { message = "Entrega realizada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
