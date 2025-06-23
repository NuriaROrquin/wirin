using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wirin.Domain.Models;
using Wirin.Domain.Services;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly OrderDeliveryService _orderDeliveryService;
    private readonly UserService _userService;

    public OrderController(OrderService orderService, OrderDeliveryService orderDeliveryService, UserService userService)
    {
        _orderService = orderService;
        _orderDeliveryService = orderDeliveryService;
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound($"Tarea con ID {id} no encontrada.");
        }
        return Ok(order);
    }

    [HttpGet("delivered")]
    [ProducesResponseType(typeof(List<OrderDelivery>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersDelivered()
    {
        var orders = await _orderDeliveryService.GetAll();
        if (orders == null)
        {
            return NotFound($"Tareas no encontradas.");
        }
        return Ok(orders);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] Order order, IFormFile file)
    {
        var trasabilityUserId = _userService.GetUserTrasabilityId(User);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string path = await GetResultFromOk(UploadFile(file));

        await _orderService.AddAsync(order, path, trasabilityUserId);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromForm] Order order, IFormFile? file)
    {
        var trasabilityUserId = _userService.GetUserTrasabilityId(User);
        if (id != order.Id)
        {
            return BadRequest("El ID proporcionado no coincide con el ID de la tarea.");
        }

        var path = (await _orderService.GetByIdAsync(id)).FilePath;

        if (file != null && file.Length != 0)
        {
            path = await GetResultFromOk(UploadFile(file));
        }


        await _orderService.UpdateAsync(order, path, trasabilityUserId);
        return NoContent(); // Indica que la operación fue exitosa pero no devuelve contenido
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderService.DeleteAsync(id);
        return NoContent();
    }

    // Guardar un archivo en la carpeta Uploads, recibiendolo con un post
    private async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No se ha proporcionado un archivo.");
        }

        string destinyFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        string result = await _orderService.SaveFile(file, destinyFolder);
        return Ok(result);
    }

    //Obtener el archivo orderId desde la carpeta Uploads, recibiendolo con un get.
    [HttpGet("download/{id}")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DownloadFile(int id)
    {
        string DestinyFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        var order = _orderService.GetByIdAsync(id).Result;

        string filePath = order.FilePath;
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("El archivo no existe.");
        }
        
        // Leer el archivo en memoria para evitar mantener el FileStream abierto
        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    [HttpGet("recovery/{id}")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RecoveryFile(int id)
    {
        string DestinyFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        var order = _orderService.GetByIdAsync(id).Result;

        string filePath = order.FilePath;
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("El archivo no existe.");
        }
        
        // Leer el archivo en memoria para evitar mantener el FileStream abierto
        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    private async Task<string?> GetResultFromOk(Task<IActionResult> action)
    {
        var result = await action;
    
        if (result is OkObjectResult okResult)
        {
            return okResult.Value?.ToString();
        }

        return null;
    }

}
