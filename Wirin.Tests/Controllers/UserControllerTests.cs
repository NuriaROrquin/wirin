using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wirin.Api.Controllers;
using Wirin.Domain.Models;
using Wirin.Domain.Services;
using Wirin.Domain.Repositories;

public class UserControllerTests
{
    private readonly Mock<UserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        var mockUserRepo = new Mock<IUserRepository>();

        // Mock con constructor dummy si UserService tiene dependencias (pasa mocks o nulls según corresponda)
        _userServiceMock = new Mock<UserService>(mockUserRepo.Object);

        _controller = new UserController(_userServiceMock.Object);

        // Setup HttpContext y User para método con [Authorize]
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user@example.com") }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsOk_WhenUserFound()
    {
        var userDomain = new User { Id = "1", Email = "user@example.com" };
        _userServiceMock.Setup(s => s.GetUserWithRoles("user@example.com"))
            .ReturnsAsync(userDomain);

        var result = await _controller.GetCurrentUser();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDomain, ok.Value);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsUnauthorized_WhenUserNotFound()
    {
        _userServiceMock.Setup(s => s.GetUserWithRoles("user@example.com"))
            .ReturnsAsync((User)null);

        var result = await _controller.GetCurrentUser();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Contains("Usuario no encontrado", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOk_WhenUsersExist()
    {
        var users = new List<User> { new User { Id = "1" }, new User { Id = "2" } };
        _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

        var result = await _controller.GetAllUsers();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, ok.Value);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsNotFound_WhenNoUsers()
    {
        _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync((List<User>)null);

        var result = await _controller.GetAllUsers();

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No se encontraron usuarios", notFound.Value.ToString());
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk_WhenUpdateSucceeds()
    {
        var userToUpdate = new User { Id = "1" };
        _userServiceMock.Setup(s => s.UpdateAsync(userToUpdate, "1")).ReturnsAsync(true);

        var result = await _controller.UpdateUser("1", userToUpdate);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Usuario actualizado", ok.Value.ToString());
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenUpdateFails()
    {
        var userToUpdate = new User { Id = "1" };
        _userServiceMock.Setup(s => s.UpdateAsync(userToUpdate, "1")).ReturnsAsync(false);

        var result = await _controller.UpdateUser("1", userToUpdate);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Error al actualizar usuario", badRequest.Value.ToString());
    }

    [Fact]
    public async Task DeleteUser_ReturnsOk_WhenDeleteSucceeds()
    {
        _userServiceMock.Setup(s => s.DeleteAsync("1")).ReturnsAsync(true);

        var result = await _controller.DeleteUser("1");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Usuario eliminado", ok.Value);
    }

    [Fact]
    public async Task DeleteUser_ReturnsBadRequest_WhenDeleteFails()
    {
        _userServiceMock.Setup(s => s.DeleteAsync("1")).ReturnsAsync(false);

        var result = await _controller.DeleteUser("1");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error al eliminar usuario", badRequest.Value);
    }

    [Fact]
    public async Task GetUserById_ReturnsOk_WhenUserFound()
    {
        var user = new User { Id = "1" };
        _userServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync(user);

        var result = await _controller.GetUserById("1");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, ok.Value);
    }

    [Fact]
    public async Task GetUserById_ReturnsNotFound_WhenUserNotFound()
    {
        _userServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync((User)null);

        var result = await _controller.GetUserById("1");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No se encontro al usuario", notFound.Value.ToString());
    }

    [Fact]
    public async Task GetUserByRole_ReturnsOk_WhenUsersFound()
    {
        var users = new List<User> { new User { Id = "1" } };
        _userServiceMock.Setup(s => s.getUsersByRoleAsync("admin")).ReturnsAsync(users);

        var result = await _controller.GetUserByRole("admin");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, ok.Value);
    }

    [Fact]
    public async Task GetUserByRole_ReturnsNotFound_WhenNoUsers()
    {
        _userServiceMock.Setup(s => s.getUsersByRoleAsync("admin")).ReturnsAsync((List<User>)null);

        var result = await _controller.GetUserByRole("admin");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No se encontraron usuarios", notFound.Value.ToString());
    }

    [Fact]
    public async Task GetAllStudentUsers_ReturnsOk_WhenStudentsFound()
    {
        var students = new List<User> { new User { Id = "1" } };
        _userServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(students);

        var result = await _controller.GetAllStudentUsers();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(students, ok.Value);
    }

    [Fact]
    public async Task GetAllStudentUsers_ReturnsNotFound_WhenNoStudents()
    {
        _userServiceMock.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync((List<User>)null);

        var result = await _controller.GetAllStudentUsers();

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No se encontraron usuarios", notFound.Value.ToString());
    }
}
