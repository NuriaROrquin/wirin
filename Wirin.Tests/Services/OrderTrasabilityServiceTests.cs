using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Xunit;

public class OrderTrasabilityServiceTests
{
    private readonly Mock<IOrderTrasabilityRepository> _orderTrasabilityRepositoryMock;
    private readonly OrderTrasabilityService _service;

    public OrderTrasabilityServiceTests()
    {
        _orderTrasabilityRepositoryMock = new Mock<IOrderTrasabilityRepository>();
        _service = new OrderTrasabilityService(_orderTrasabilityRepositoryMock.Object);
    }
    [Fact]
    public async Task GetAllOrderTrasabilities_ReturnsList()
    {
        // Arrange
        var expectedList = new List<OrderTrasability>
    {
        new OrderTrasability { OrderId = 1, Action = "Creado", UserId = "user1", ProcessedAt = DateTime.UtcNow },
        new OrderTrasability { OrderId = 2, Action = "Actualizado", UserId = "user2", ProcessedAt = DateTime.UtcNow }
    };
        _orderTrasabilityRepositoryMock
            .Setup(repo => repo.GetAllOrderTrasabilities())
            .ReturnsAsync(expectedList);

        // Act
        var result = await _service.GetAllOrderTrasabilities();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedList.Count, result.Count);
        Assert.Equal(expectedList[0].Action, result[0].Action);
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByOrderIdAsync_ReturnsList()
    {
        // Arrange
        int orderId = 5;
        var expectedList = new List<OrderTrasability>
        {
            new OrderTrasability { OrderId = orderId, Action = "Creado" }
        };
        _orderTrasabilityRepositoryMock
            .Setup(repo => repo.GetOrderTrasabilitiesByOrderIdAsync(orderId))
            .ReturnsAsync(expectedList);

        // Act
        var result = await _service.GetOrderTrasabilitiesByOrderIdAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, item => Assert.Equal(orderId, item.OrderId));
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByActionAsync_ReturnsList()
    {
        // Arrange
        string action = "Creado";
        var expectedList = new List<OrderTrasability>
        {
            new OrderTrasability { Action = action }
        };
        _orderTrasabilityRepositoryMock
            .Setup(repo => repo.GetOrderTrasabilitiesByActionAsync(action))
            .ReturnsAsync(expectedList);

        // Act
        var result = await _service.GetOrderTrasabilitiesByActionAsync(action);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, item => Assert.Equal(action, item.Action));
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByUserAsync_ReturnsList()
    {
        // Arrange
        string userId = "user123";
        var expectedList = new List<OrderTrasability>
        {
            new OrderTrasability { UserId = userId }
        };
        _orderTrasabilityRepositoryMock
            .Setup(repo => repo.GetOrderTrasabilitiesByUserAsync(userId))
            .ReturnsAsync(expectedList);

        // Act
        var result = await _service.GetOrderTrasabilitiesByUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, item => Assert.Equal(userId, item.UserId));
    }
}
