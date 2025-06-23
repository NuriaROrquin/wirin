using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wirin.Api.Controllers;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Services;
using Wirin.Domain.Services;
using Wirin.Domain.Repositories;
using Wirin.Domain.Providers;
using Wirin.Domain.Dtos.Request;

public class OrderManagmentControllerTests
{
    private readonly Mock<OrderManagmentService> _orderManagmentServiceMock;
    private readonly Mock<UserService> _userServiceMock;
    private readonly OrderManagmentController _controller;

    public OrderManagmentControllerTests()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderParagraphRepo = new Mock<IOrderParagraphRepository>();
        var mockOrderManagmentRepo = new Mock<IOrderManagmentRepository>();
        var mockOrderTrasabilitiRepo = new Mock<IOrderTrasabilityRepository>();
        var mockOrderDeliveryRepo = new Mock<IOrderDeliveryRepository>();
        var mockOrderSequenceRepo = new Mock<IOrderSequenceRepository>();
        var mockUserRepo = new Mock<IUserRepository>();


        _orderManagmentServiceMock = new Mock<OrderManagmentService>(mockOrderParagraphRepo.Object, mockOrderManagmentRepo.Object, mockOrderRepo.Object, mockOrderDeliveryRepo.Object, mockOrderSequenceRepo.Object, mockOrderTrasabilitiRepo.Object);
        _userServiceMock = new Mock<UserService>(mockUserRepo.Object);
        _controller = new OrderManagmentController(_orderManagmentServiceMock.Object, _userServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetAllOrderByStatus_ReturnsOk()
    {
        var orders = new List<Order> { new Order { Id = 1 } };
        _orderManagmentServiceMock.Setup(s => s.GetAllOrderByStatus("Pendiente")).ReturnsAsync(orders);

        var result = await _controller.GetAllOrderByStatus("Pendiente");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orders, ok.Value);
    }

    [Fact]
    public async Task GetAllByUserAssigned_ReturnsOk()
    {
        var orders = new List<Order> { new Order { Id = 1 } };
        _orderManagmentServiceMock.Setup(s => s.GetAllByUserAssigned("voluntario123")).ReturnsAsync(orders);

        var result = await _controller.GetAllByUserAssigned("voluntario123");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orders, ok.Value);
    }

    [Fact]
    public async Task ChangeState_ReturnsOk()
    {
        var request = new OrderChangeStatusRequest { Id = 1, Status = "Revisado" };
        _userServiceMock.Setup(s => s.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns("user123");

        _orderManagmentServiceMock.Setup(s => s.ChangeState(1, "Revisado", "user123")).Returns(Task.CompletedTask);

        var result = await _controller.ChangeState(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Tarea actualizada", ok.Value.ToString());
    }

    [Fact]
    public async Task SaveVoluntarioId_ReturnsOk()
    {
        var request = new OrderSaveAssignedUserId { Id = 1, userId = "voluntario123" };
        _userServiceMock.Setup(s => s.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns("user123");

        _orderManagmentServiceMock.Setup(s => s.SaveVoluntarioId(1, "voluntario123", "user123")).Returns(Task.CompletedTask);

        var result = await _controller.SaveVoluntarioId(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Voluntario actualizado", ok.Value.ToString());
    }

    [Fact]
    public async Task SaveRevisorId_ReturnsOk()
    {
        var request = new OrderSaveAssignedUserId { Id = 1, userId = "revisor123" };
        _userServiceMock.Setup(s => s.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns("user123");

        _orderManagmentServiceMock.Setup(s => s.SaveRevisorId(1, "revisor123", "user123")).Returns(Task.CompletedTask);

        var result = await _controller.SaveRevisorId(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Revisor actualizado", ok.Value.ToString());
    }

    [Fact]
    public async Task GetAllByStudent_ReturnsNotFound_WhenEmpty()
    {
        var emptyOrders = new List<OrderWithParagraphs>();

        _orderManagmentServiceMock
            .Setup(s => s.GetAllByStudent("student123"))
            .ReturnsAsync(emptyOrders);

        var result = await _controller.GetAllByStudent("student123");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("no se encontraron", notFound.Value.ToString().ToLower());
    }
    [Fact]
    public async Task GetAllByStudent_ReturnsOk_WhenFound()
    {
        var orders = new List<OrderWithParagraphs>
    {
        new OrderWithParagraphs
        {
            Order = new Order { Id = 1 },
            ParagraphTexts = new List<Paragraph>
            {
                new Paragraph { OrderId = 1, ParagraphText = "Párrafo 1", PageNumber = 1, Confidence = 0.95 },
                new Paragraph { OrderId = 1, ParagraphText = "Párrafo 2", PageNumber = 2, Confidence = 0.9 }
            }
        }
    };

        _orderManagmentServiceMock
            .Setup(s => s.GetAllByStudent("student123"))
            .ReturnsAsync(orders);

        var result = await _controller.GetAllByStudent("student123");

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OrderWithParagraphs>>(ok.Value);

        // Comprobar que la lista tiene la cantidad esperada
        Assert.Single(value);

        // Comprobar detalles del primer elemento
        Assert.Equal(1, value[0].Order.Id);
        Assert.Equal(2, value[0].ParagraphTexts.Count);
        Assert.Equal("Párrafo 1", value[0].ParagraphTexts[0].ParagraphText);
        Assert.Equal("Párrafo 2", value[0].ParagraphTexts[1].ParagraphText);
    }


}
