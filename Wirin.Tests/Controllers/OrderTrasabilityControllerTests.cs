using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wirin.Api.Controllers;
using Wirin.Domain.Services;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

public class OrderTrasabilityControllerTests
{
    private readonly Mock<OrderTrasabilityService> _orderTrasabilityServiceMock;
    private readonly OrderTrasabilityController _controller;

    public OrderTrasabilityControllerTests()
    {
        var mockOrderTrasabilitiRepo = new Mock<IOrderTrasabilityRepository>();
        _orderTrasabilityServiceMock = new Mock<OrderTrasabilityService>(mockOrderTrasabilitiRepo.Object);
        _controller = new OrderTrasabilityController(_orderTrasabilityServiceMock.Object);
    }

    [Fact]
    public async Task GetTrazabilitys_ReturnsOk_WithData()
    {
        // Arrange
        var data = new List<OrderTrasability> { new OrderTrasability { OrderId = 1, UserId = "user1", Action = "Action1" } };
        _orderTrasabilityServiceMock.Setup(s => s.GetAllOrderTrasabilities())
            .ReturnsAsync(data);

        // Act
        var result = await _controller.GetTrazabilitys();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseList = Assert.IsAssignableFrom<IEnumerable<OrderTrasabilityResponse>>(okResult.Value);
    }

    [Fact]
    public async Task GetTrazabilitys_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        _orderTrasabilityServiceMock.Setup(s => s.GetAllOrderTrasabilities())
            .ReturnsAsync((List<OrderTrasability>)null);

        // Act
        var result = await _controller.GetTrazabilitys();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("Sin Datos de Trazabilidad", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByOrderIdAsync_ReturnsOk_WithData()
    {
        // Arrange
        var data = new List<OrderTrasability> { new OrderTrasability { OrderId = 5, UserId = "user1", Action = "Action1" } };
        _orderTrasabilityServiceMock.Setup(s => s.GetOrderTrasabilitiesByOrderIdAsync(5))
            .ReturnsAsync(data);

        // Act
        var result = await _controller.GetOrderTrasabilitiesByOrderIdAsync(5);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseList = Assert.IsAssignableFrom<IEnumerable<OrderTrasabilityResponse>>(okResult.Value);
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByOrderIdAsync_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        _orderTrasabilityServiceMock.Setup(s => s.GetOrderTrasabilitiesByOrderIdAsync(5))
            .ReturnsAsync((List<OrderTrasability>)null);

        // Act
        var result = await _controller.GetOrderTrasabilitiesByOrderIdAsync(5);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("Sin Datos de Trazabilidad para este pedido", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByActionAsync_ReturnsOk_WithData()
    {
        // Arrange
        var action = "TestAction";
        var data = new List<OrderTrasability> { new OrderTrasability { OrderId = 2, UserId = "user2", Action = action } };
        _orderTrasabilityServiceMock.Setup(s => s.GetOrderTrasabilitiesByActionAsync(action))
            .ReturnsAsync(data);

        // Act
        var result = await _controller.GetOrderTrasabilitiesByActionAsync(action);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseList = Assert.IsAssignableFrom<IEnumerable<OrderTrasabilityResponse>>(okResult.Value);
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByActionAsync_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        _orderTrasabilityServiceMock.Setup(s => s.GetOrderTrasabilitiesByActionAsync("NoAction"))
            .ReturnsAsync((List<OrderTrasability>)null);

        // Act
        var result = await _controller.GetOrderTrasabilitiesByActionAsync("NoAction");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("Sin Datos de Trazabilidad para esta acción", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByUserAsync_ReturnsOk_WithData()
    {
        // Arrange
        var userId = "user123";
        var data = new List<OrderTrasability> { new OrderTrasability { OrderId = 3, UserId = userId, Action = "Action3" } };
        _orderTrasabilityServiceMock.Setup(s => s.GetOrderTrasabilitiesByUserAsync(userId))
            .ReturnsAsync(data);

        // Act
        var result = await _controller.GetOrderTrasabilitiesByUserAsync(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseList = Assert.IsAssignableFrom<IEnumerable<OrderTrasabilityResponse>>(okResult.Value);
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByUserAsync_ReturnsNotFound_WhenNoData()
    {
        _orderTrasabilityServiceMock.Setup(s => s.GetOrderTrasabilitiesByUserAsync("unknownUser"))
            .ReturnsAsync((List<OrderTrasability>)null);

        var result = await _controller.GetOrderTrasabilitiesByUserAsync("unknownUser");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var messageProperty = notFoundResult.Value.GetType().GetProperty("message");
        Assert.NotNull(messageProperty);
        var messageValue = messageProperty.GetValue(notFoundResult.Value) as string;

        Assert.Equal("Sin Datos de Trazabilidad para este usuario", messageValue);
    }

}
