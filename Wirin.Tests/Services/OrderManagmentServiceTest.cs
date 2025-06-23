using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Wirin.Infrastructure.Services;
using Xunit;
using static NuGet.Packaging.PackagingConstants;

public class OrderManagmentServiceTests
{
    private readonly Mock<IOrderManagmentRepository> _orderManagmentRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderDeliveryRepository> _orderDeliveryRepositoryMock;
    private readonly Mock<IOrderSequenceRepository> _orderSequenceRepositoryMock;
    private readonly Mock<IOrderTrasabilityRepository> _orderTrasabilityRepositoryMock;
    private readonly Mock<IOrderParagraphRepository> _orderParagraphRepositoryMock;

    private readonly OrderManagmentService _sut;

    public OrderManagmentServiceTests()
    {
        _orderManagmentRepositoryMock = new Mock<IOrderManagmentRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderDeliveryRepositoryMock = new Mock<IOrderDeliveryRepository>();
        _orderSequenceRepositoryMock = new Mock<IOrderSequenceRepository>();
        _orderTrasabilityRepositoryMock = new Mock<IOrderTrasabilityRepository>();
        _orderParagraphRepositoryMock = new Mock<IOrderParagraphRepository>();

        _sut = new OrderManagmentService(
            _orderParagraphRepositoryMock.Object,
            _orderManagmentRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _orderDeliveryRepositoryMock.Object,
            _orderSequenceRepositoryMock.Object,
            _orderTrasabilityRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllOrderByStatus_ReturnsOrders()
    {
        var status = "Pendiente";
        var orders = new List<Order> {
            new Order { Id = 1, Status = status },
            new Order { Id = 2, Status = status }
        };
        _orderManagmentRepositoryMock.Setup(x => x.GetAllOrderByStatus(status)).ReturnsAsync(orders);

        var result = await _sut.GetAllOrderByStatus(status);

        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal(status, o.Status));
    }

    [Fact]
    public async Task SaveRevisorId_UpdatesOrderAndSavesTrasability()
    {
        var orderId = 1;
        var revisorId = "rev123";
        var trasabilityUserId = "user456";

        var order = new Order { Id = orderId };
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
        _orderManagmentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _orderTrasabilityRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<OrderTrasability>())).Returns(Task.CompletedTask);

        await _sut.SaveRevisorId(orderId, revisorId, trasabilityUserId);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderManagmentRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Order>(o => o.RevisorId == revisorId)), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(x => x.SaveAsync(It.Is<OrderTrasability>(t => t.UserId == trasabilityUserId && t.Action.Contains("Asignado a voluntario"))), Times.Once);
    }

    [Fact]
    public async Task SaveVoluntarioId_UpdatesOrderAndSavesTrasability()
    {
        var orderId = 1;
        var voluntarioId = "vol123";
        var trasabilityUserId = "user456";

        var order = new Order { Id = orderId };
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
        _orderManagmentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _orderTrasabilityRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<OrderTrasability>())).Returns(Task.CompletedTask);

        await _sut.SaveVoluntarioId(orderId, voluntarioId, trasabilityUserId);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderManagmentRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Order>(o => o.VoluntarioId == voluntarioId)), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(x => x.SaveAsync(It.Is<OrderTrasability>(t => t.UserId == trasabilityUserId && t.Action.Contains("Asignado a voluntario"))), Times.Once);
    }

    [Fact]
    public async Task ChangeState_UpdatesOrderStatusAndSavesTrasability()
    {
        var orderId = 1;
        var newState = "En Revisión";
        var trasabilityUserId = "user123";

        var order = new Order { Id = orderId, Status = "En Proceso" };
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
        _orderManagmentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _orderTrasabilityRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<OrderTrasability>())).Returns(Task.CompletedTask);

        await _sut.ChangeState(orderId, newState, trasabilityUserId);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderManagmentRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Order>(o => o.Status == newState)), Times.Once);
        _orderTrasabilityRepositoryMock.Verify(x => x.SaveAsync(It.Is<OrderTrasability>(t => t.UserId == trasabilityUserId && t.Action.Contains(newState))), Times.Once);
    }

    [Fact]
    public async Task GetAllByStudent_ReturnsOrdersWithParagraphTexts()
    {
        // Arrange
        var studentId = "student123";

        var orders = new List<Order>
        {
            new Order { Id = 1 },
            new Order { Id = 2 }
        };

        var paragraphsOrder1 = new List<Paragraph>
        {
            new Paragraph { ParagraphText = "P1" },
            new Paragraph { ParagraphText = "P2" }
        };

        var paragraphsOrder2 = new List<Paragraph>
        {
            new Paragraph { ParagraphText = "P3" }
        };

        _orderManagmentRepositoryMock
            .Setup(x => x.GetAllByStudentAsync(studentId))
            .ReturnsAsync(orders);

        _orderParagraphRepositoryMock
            .Setup(x => x.GetAllParagraphsByOrderIdAsync(1))
            .ReturnsAsync(paragraphsOrder1);

        _orderParagraphRepositoryMock
            .Setup(x => x.GetAllParagraphsByOrderIdAsync(2))
            .ReturnsAsync(paragraphsOrder2);

        // Act
        var result = await _sut.GetAllByStudent(studentId);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal("P1", result[0].ParagraphTexts[0].ParagraphText);
        Assert.Equal("P2", result[0].ParagraphTexts[1].ParagraphText);

        Assert.Single(result[1].ParagraphTexts);
        Assert.Equal("P3", result[1].ParagraphTexts[0].ParagraphText);
    }

}
