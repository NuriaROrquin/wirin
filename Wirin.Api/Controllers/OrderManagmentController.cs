using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Services;
using Wirin.Infrastructure.Services;

namespace Wirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderManagmentController : ControllerBase
    {

        private readonly OrderManagmentService _OrderManagmentService;
        private readonly UserService _userService;

        
        public OrderManagmentController(OrderManagmentService OrderManagmentService, UserService userService)
        {
            _OrderManagmentService = OrderManagmentService;
            _userService = userService;
        }



        [HttpGet("Bystatus")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrderByStatus(string status)
        {
 

            var orders = await _OrderManagmentService.GetAllOrderByStatus(status);
            return Ok(orders);
        }

        [HttpGet("byAssigned")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByUserAssigned(string UserId)
        {

            var orders = await _OrderManagmentService.GetAllByUserAssigned(UserId);
            return Ok(orders);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeState([FromBody] OrderChangeStatusRequest request)
        {
            var trasabilityUserId = _userService.GetUserTrasabilityId(User);

            try
            {

                await _OrderManagmentService.ChangeState(request.Id, request.Status, trasabilityUserId);
                return Ok(new { message = "Tarea actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("saveVoluntarioId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveVoluntarioId([FromBody] OrderSaveAssignedUserId request)
        {
            var trasabilityUserId = _userService.GetUserTrasabilityId(User);
            try
            {

                await _OrderManagmentService.SaveVoluntarioId(request.Id, request.userId, trasabilityUserId);
                return Ok(new { message = "Voluntario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("saveRevisorId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveRevisorId([FromBody] OrderSaveAssignedUserId request)
        {
            var trasabilityUserId = _userService.GetUserTrasabilityId(User);
            try
            {

                await _OrderManagmentService.SaveRevisorId(request.Id, request.userId, trasabilityUserId);
                return Ok(new { message = "Revisor actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

        [HttpGet("byStudent/{studentId}")]
        [ProducesResponseType(typeof(List<Order>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllByStudent([FromRoute] string studentId)
        {
            var orders = await _OrderManagmentService.GetAllByStudent(studentId);

            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No se encontraron tareas." });
            }

            return Ok(orders);
        }

    }

}