using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wirin.Api.Controllers;
using Wirin.Domain.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Services;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Repositories;

public class OrderDeliveryControllerTests
{
    private readonly Mock<OrderDeliveryService> _deliveryServiceMock;
    private readonly Mock<UserService> _userServiceMock;
    private readonly OrderDeliveryController _controller;

    public OrderDeliveryControllerTests()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderTrasabilitiRepo = new Mock<IOrderTrasabilityRepository>();
        var mockOrderDeliveryRepo = new Mock<IOrderDeliveryRepository>();
        var mockOrderSequenceRepo = new Mock<IOrderSequenceRepository>();
        var mockUserRepo = new Mock<IUserRepository>();

        _deliveryServiceMock = new Mock<OrderDeliveryService>(mockOrderDeliveryRepo.Object, mockOrderRepo.Object, mockOrderSequenceRepo.Object, mockOrderTrasabilitiRepo.Object, mockUserRepo.Object);
        _userServiceMock = new Mock<UserService>(mockUserRepo.Object);

        _controller = new OrderDeliveryController(_deliveryServiceMock.Object, _userServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetOrderDeliveries_ReturnsAllOrderDeliveries()
    {
        var expected = new List<OrderDelivery> { new OrderDelivery { Id = 1 } };
        _deliveryServiceMock.Setup(s => s.GetAll()).ReturnsAsync(expected);

        var result = await _controller.GetOrderDeliveries();

        var ok = Assert.IsType<ActionResult<IEnumerable<OrderDelivery>>>(result);
        Assert.Equal(expected, ok.Value);
    }

    [Fact]
    public async Task GetOrderDeliveriesWithOrders_ReturnsOk()
    {
        var deliveries = new List<OrderDelivery> { new OrderDelivery { Id = 1 } };
        _deliveryServiceMock.Setup(s => s.GetAllWithOrders()).ReturnsAsync(deliveries);

        var result = await _controller.GetOrderDeliveriesWithOrders();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(deliveries, ok.Value);
    }

    [Fact]
    public async Task GetOrderDeliveriesWithOrdersByStudentId_ReturnsOk()
    {
        var deliveries = new List<OrderDelivery> { new OrderDelivery { Id = 1 } };
        _deliveryServiceMock.Setup(s => s.GetAllWithOrdersByStudentId("student123"))
            .ReturnsAsync(deliveries);

        var result = await _controller.GetOrderDeliveriesWithOrdersByStudentId("student123");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(deliveries, ok.Value);
    }



    [Fact]
    public async Task PostPerformDelivery_ReturnsOk()
    {
        var request = new PerformDeliveryRequest { SelectedOrders = new(), StudentId = "student123" };
        _userServiceMock.Setup(x => x.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns("user123");
        _deliveryServiceMock.Setup(x => x.PerformDeliveryAsync(request, "user123"))
            .Returns(Task.CompletedTask);

        var result = await _controller.PostPerformDelivery(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("correctamente", ok.Value.ToString());
    }

    [Fact]
    public async Task PostOrderDeliveryEntity_CreatesOrderDelivery()
    {
        var request = new CreateOrderDeliveryRequest
        {
            Title = "Entrega 1",
            Status = "Nueva",
            StudentId = "student123"
        };

        _userServiceMock.Setup(x => x.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns("user123");
        _deliveryServiceMock.Setup(x => x.Create(It.IsAny<OrderDelivery>())).Returns(Task.CompletedTask);

        await _controller.PostOrderDeliveryEntity(request);

        _deliveryServiceMock.Verify(x => x.Create(It.Is<OrderDelivery>(od =>
            od.Title == request.Title &&
            od.Status == request.Status &&
            od.StudentId == request.StudentId &&
            od.UserId == "user123"
        )), Times.Once);
    }
}