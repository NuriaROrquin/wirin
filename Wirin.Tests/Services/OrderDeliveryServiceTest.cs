using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Xunit;

public class OrderDeliveryServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderSequenceRepository> _orderSequenceRepositoryMock;
    private readonly Mock<IOrderTrasabilityRepository> _orderTrasabilityRepositoryMock;
    private readonly Mock<IOrderDeliveryRepository> _orderDeliveryRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly OrderDeliveryService _sut; // System Under Test

    public OrderDeliveryServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderSequenceRepositoryMock = new Mock<IOrderSequenceRepository>();
        _orderTrasabilityRepositoryMock = new Mock<IOrderTrasabilityRepository>();
        _orderDeliveryRepositoryMock = new Mock<IOrderDeliveryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _sut = new OrderDeliveryService(
            _orderDeliveryRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _orderSequenceRepositoryMock.Object,
            _orderTrasabilityRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_Returns_ListOfDeliveries()
    {
        // Arrange
        var expected = new List<OrderDelivery> { new OrderDelivery { Id = 1 }, new OrderDelivery { Id = 2 } };
        _orderDeliveryRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(expected);

        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.Equal(expected.Count, result.Count);
        Assert.Equal(expected[0].Id, result[0].Id);
    }

    [Fact]
    public async Task GetAllWithOrders_SetsUserNames()
    {
        // Arrange
        var deliveries = new List<OrderDelivery>
        {
            new OrderDelivery { Id = 1, UserId = "user1", StudentId = "student1" }
        };

        _orderDeliveryRepositoryMock.Setup(r => r.GetAllWithOrders()).ReturnsAsync(deliveries);
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync("user1"))
            .ReturnsAsync(new User { FullName = "User One" });
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync("student1"))
            .ReturnsAsync(new User { FullName = "Student One" });

        // Act
        var result = await _sut.GetAllWithOrders();

        // Assert
        Assert.Single(result);
        Assert.Equal("User One", result[0].UserName);
        Assert.Equal("Student One", result[0].StudentUserName);
    }

    [Fact]
    public async Task PerformDeliveryAsync_WhenOrderNotFound_ThrowsException()
    {
        // Arrange
        var request = new PerformDeliveryRequest
        {
            StudentId = "student1",
            SelectedOrders = new List<OrderSequence>
            {
                new OrderSequence { OrderId = 123 }
            }
        };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Order)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _sut.PerformDeliveryAsync(request, "user1"));
        Assert.Equal("Orden no encontrada.", ex.Message);
    }

    [Fact]
    public async Task PerformDeliveryAsync_PerformsFullDeliveryProcess()
    {
        // Arrange
        var selectedOrders = new List<OrderSequence>
        {
            new OrderSequence { OrderId = 1 },
            new OrderSequence { OrderId = 2 }
        };

        var request = new PerformDeliveryRequest
        {
            StudentId = "student1",
            SelectedOrders = selectedOrders
        };

        // Orders exist
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Order { Id = 1 });

        // AddNewDeliveryAsync returns a new OrderDelivery with Id
        _orderDeliveryRepositoryMock.Setup(r => r.AddNewDeliveryAsync("student1", "user1"))
            .ReturnsAsync(new OrderDelivery { Id = 10 });

        _orderSequenceRepositoryMock.Setup(r => r.PerformDelivery(selectedOrders, 10))
            .Returns(Task.CompletedTask);

        _orderTrasabilityRepositoryMock.Setup(r => r.SaveAsync(It.IsAny<OrderTrasability>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.PerformDeliveryAsync(request, "user1");

        // Assert
        _orderRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Exactly(selectedOrders.Count));
        _orderDeliveryRepositoryMock.Verify(r => r.AddNewDeliveryAsync("student1", "user1"), Times.Once);
        _orderSequenceRepositoryMock.Verify(r => r.PerformDelivery(selectedOrders, 10), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<OrderTrasability>()), Times.Once);
    }
}
