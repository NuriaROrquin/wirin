using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentDeliveryController : ControllerBase
    {
        private readonly StudentDeliveryService _studentDeliveryService;
        private readonly UserService _userService;
        public StudentDeliveryController(StudentDeliveryService studentDeliveryService, UserService userService)
        {
            _studentDeliveryService = studentDeliveryService;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<StudentDelivery>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudentDeliveriesAsync()
        {

            var deliveries = await _studentDeliveryService.GetStudentDeliveriesAsync();

            return Ok(deliveries);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create( StudentDeliveryRequest request)
        {
            var studentDelivery = new StudentDelivery
            {
                StudentId = request.StudentId,
                OrderDeliveryId = request.OrderDeliveryId,
                CreateDate = request.CreateDate ?? DateTime.UtcNow
            };

            if (request == null)
            {
                return BadRequest("El objeto de entrega del estudiante es nulo.");
            }

            await _studentDeliveryService.CreateStudentDelivery(studentDelivery);
            return Ok(new { message = "Entrega del estudiante creada correctamente." });

        }

        [HttpGet("{orderDeliveryId}")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersWithoutOrderDelivery(int orderDeliveryId)
        {
            var students = await _userService.GetAllStudentsAsync();

            if (orderDeliveryId != 0)
            {
                students = await _studentDeliveryService.GetUsersWithoutOrderDelivery(students, orderDeliveryId);
            }

            if (students == null)
            {
                return NotFound(new { message = "No se encontraron usuarios." });
            }

            return Ok(students);
        }

    }
}
