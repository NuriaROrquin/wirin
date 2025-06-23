using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wirin.Api.Controllers;
using Wirin.Domain.Services;
using Wirin.Domain.Models;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Repositories;
using Wirin.Api.Dtos.Response;

public class AuthControllerTests
{
    private readonly Mock<UserService> _userServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthController _controller;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AuthControllerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userServiceMock = new Mock<UserService>(_userRepositoryMock.Object);
        _configurationMock = new Mock<IConfiguration>();

        _controller = new AuthController(_userServiceMock.Object, _configurationMock.Object);
       

    }

    [Fact]
    public async Task Login_ValidUser_ReturnsOkWithToken()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "correctpassword"
        };

        var user = new User
        {
            Id = "1",
            Email = "test@test.com",
            UserName = "testuser",
            FullName = "Test User",
            PhoneNumber = "12345",
            Roles = new List<string> { "User" }
        };


        _userServiceMock.Setup(u => u.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);

        _userServiceMock.Setup(u => u.CheckPasswordAsync(user, loginRequest.Password))
            .ReturnsAsync(true); // Contraseña válida

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["AppConfig:Jwt:Key"]).Returns("12345678901234567890123456789012"); // 32 chars
        mockConfig.Setup(c => c["AppConfig:Jwt:Issuer"]).Returns("testIssuer");
        mockConfig.Setup(c => c["AppConfig:Jwt:Audience"]).Returns("testAudience");

        var controller = new AuthController(_userServiceMock.Object, mockConfig.Object);

        // Act
        var result = await controller.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.NotEmpty(response.token);
        Assert.Equal(user.Id, response.userId);
    }


    [Theory]
    [InlineData("wrongpassword")]
    public async Task Login_InvalidUserOrPassword_ReturnsUnauthorized(string password)
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "user@test.com",
            Password = password
        };

        _userServiceMock.Setup(u => u.GetByEmailAsync(loginRequest.Email))
                        .ReturnsAsync(new User { Id = "1", Email = "user@test.com", Password = "hashed" });

        _userServiceMock.Setup(u => u.CheckPasswordAsync(It.IsAny<User>(), loginRequest.Password))
                        .ReturnsAsync(false); // Password incorrecto

        _configurationMock.Setup(c => c["AppConfig:Jwt:Key"]).Returns("12345678901234567890123456789012");
        _configurationMock.Setup(c => c["AppConfig:Jwt:Issuer"]).Returns("issuer");
        _configurationMock.Setup(c => c["AppConfig:Jwt:Audience"]).Returns("audience");

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    [Fact]
    public async Task Login_MissingJwtConfig_Returns500()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "correctpassword"
        };

        var user = new User
        {
            Id = "1",
            Email = "test@test.com",
            UserName = "testuser",
            Roles = new List<string> { "User" }
        };

        _userServiceMock.Setup(u => u.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);

        _userServiceMock.Setup(u => u.CheckPasswordAsync(user, loginRequest.Password))
            .ReturnsAsync(true);

        // Simula que las configuraciones JWT faltan
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["AppConfig:Jwt:Key"]).Returns((string)null);
        mockConfig.Setup(c => c["AppConfig:Jwt:Issuer"]).Returns(string.Empty);
        mockConfig.Setup(c => c["AppConfig:Jwt:Audience"]).Returns((string)null);

        var controller = new AuthController(_userServiceMock.Object, mockConfig.Object);

        // Act
        var result = await controller.Login(loginRequest);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }




    [Fact]
    public async Task Register_EmailAlreadyExists_ReturnsBadRequest()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "test@test.com",
            UserName = "testuser",
            Password = "password",
            FullName = "Test User",
            PhoneNumber = "12345",
            Roles = new List<string> { "User" }
        };

        _userServiceMock.Setup(u => u.GetByEmailAsync(registerRequest.Email)).ReturnsAsync(new User());

        var result = await _controller.Register(registerRequest);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_UserNameAlreadyExists_ReturnsBadRequest()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "test@test.com",
            UserName = "testuser",
            Password = "password",
            FullName = "Test User",
            PhoneNumber = "12345",
            Roles = new List<string> { "User" }
        };

        _userServiceMock.Setup(u => u.GetByEmailAsync(registerRequest.Email)).ReturnsAsync((User)null);
        _userServiceMock.Setup(u => u.GetUserByUserNameAsync(registerRequest.UserName)).ReturnsAsync(new User());

        var result = await _controller.Register(registerRequest);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_Success_ReturnsOk()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "test@test.com",
            UserName = "testuser",
            Password = "password",
            FullName = "Test User",
            PhoneNumber = "12345",
            Roles = new List<string> { "User" }
        };

        _userServiceMock.Setup(u => u.GetByEmailAsync(registerRequest.Email)).ReturnsAsync((User)null);
        _userServiceMock.Setup(u => u.GetUserByUserNameAsync(registerRequest.UserName)).ReturnsAsync((User)null);
        _userServiceMock.Setup(u => u.AddUserAsync(It.IsAny<User>(), registerRequest.Password)).ReturnsAsync(true);

        var result = await _controller.Register(registerRequest);

        Assert.IsType<OkObjectResult>(result);
    }
}
