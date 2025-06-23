using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Services;

namespace Wirin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var email = User.Identity.Name;
        var user = await _userService.GetUserWithRoles(email);
       
        if (user == null)
        {
            return Unauthorized(new { message = "Usuario no encontrado." });
        }

        
        return Ok(user);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        if (users == null)
        {
            return NotFound(new { message = "No se encontraron usuarios." });
        }

        return Ok(users);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
    {

        var updated = await _userService.UpdateAsync(user, id);
        return updated
            ? Ok(new { message = "Usuario actualizado" })
            : BadRequest(new { error = "Error al actualizar usuario" });

    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var deleted = await _userService.DeleteAsync(id);
        return deleted ? Ok("Usuario eliminado") : BadRequest("Error al eliminar usuario");
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound(new { message = "No se encontro al usuario." });
        }

        return Ok(user);
    }

    [HttpGet("by-role/{role}")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByRole(string role)
    {
        var user = await _userService.getUsersByRoleAsync(role);
        if (user == null)
        {
            return NotFound(new { message = "No se encontraron usuarios." });
        }

        return Ok(user);
    }

    [HttpGet("students")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllStudentUsers()
    {
        var users = await _userService.GetAllStudentsAsync();
        if (users == null)
        {
            return NotFound(new { message = "No se encontraron usuarios." });
        }

        return Ok(users);
    }
}
