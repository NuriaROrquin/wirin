using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User { Id = "1", Email = "user1@test.com" },
            new User { Id = "2", Email = "user2@test.com" }
        };
        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedUsers);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Count, result.Count());
    }

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        var userId = "123";
        var user = new User { Id = userId, Email = "email@test.com" };
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

        var result = await _service.GetUserById(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task CheckPasswordAsync_ReturnsTrue()
    {
        var user = new User { Id = "1" };
        var password = "password123";

        _userRepositoryMock.Setup(r => r.CheckPasswordAsync(user, password)).ReturnsAsync(true);

        var result = await _service.CheckPasswordAsync(user, password);

        Assert.True(result);
    }

    [Fact]
    public async Task AddUserAsync_ReturnsTrue()
    {
        var user = new User { Id = "1", Email = "test@test.com" };
        var password = "password";

        _userRepositoryMock.Setup(r => r.AddUserAsync(user, password)).ReturnsAsync(true);

        var result = await _service.AddUserAsync(user, password);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue()
    {
        var user = new User { Id = "1" };
        var id = "1";

        _userRepositoryMock.Setup(r => r.UpdateAsync(user, id)).ReturnsAsync(true);

        var result = await _service.UpdateAsync(user, id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue()
    {
        var id = "1";

        _userRepositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(id);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUsersByRoleAsync_ReturnsUsers()
    {
        var role = "Student";
        var users = new List<User>
        {
            new User { Id = "1", Email = "student1@test.com" },
            new User { Id = "2", Email = "student2@test.com" }
        };

        _userRepositoryMock.Setup(r => r.GetUsersByRol(role)).ReturnsAsync(users);

        var result = await _service.getUsersByRoleAsync(role);

        Assert.NotNull(result);
        Assert.Equal(users.Count, result.Count());
    }

    [Fact]
    public void GetUserTrasabilityId_ReturnsUserId()
    {
        // Arrange
        var email = "email@test.com";
        var userId = "user-id-123";

        var user = new User { Id = userId, Email = email };

        _userRepositoryMock.Setup(r => r.GetUserWithRoles(email)).ReturnsAsync(user);

        var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Act
        var result = _service.GetUserTrasabilityId(claimsPrincipal);

        // Assert
        Assert.Equal(userId, result);
    }
}
