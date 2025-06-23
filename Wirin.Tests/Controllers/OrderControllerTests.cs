using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wirin.Domain.Services;
using Wirin.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using Wirin.Domain.Providers;
using Wirin.Domain.Repositories;

public class OrderControllerTests
{
    private readonly Mock<OrderService> _orderServiceMock;
    private readonly Mock<OrderDeliveryService> _deliveryServiceMock;
    private readonly Mock<UserService> _userServiceMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderTrasabilitiRepo = new Mock<IOrderTrasabilityRepository>();
        var mockOrderDeliveryRepo = new Mock<IOrderDeliveryRepository>();
        var mockOrderSequenceRepo = new Mock<IOrderSequenceRepository>();
        var mockUserRepo = new Mock<IUserRepository>();

        _orderServiceMock = new Mock<OrderService>(mockOrderRepo.Object, mockOrderTrasabilitiRepo.Object, mockOrderSequenceRepo.Object);
        _deliveryServiceMock = new Mock<OrderDeliveryService>(mockOrderDeliveryRepo.Object, mockOrderRepo.Object, mockOrderSequenceRepo.Object, mockOrderTrasabilitiRepo.Object, mockUserRepo.Object);
        _userServiceMock = new Mock<UserService>(mockUserRepo.Object);

        _controller = new OrderController(
            _orderServiceMock.Object,
            _deliveryServiceMock.Object,
            _userServiceMock.Object
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var orders = new List<Order> { new Order { Id = 1 } };
        _orderServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(orders);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orders, ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var order = new Order { Id = 1 };
        _orderServiceMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNull()
    {
        _orderServiceMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Order)null);

        var result = await _controller.GetById(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("no encontrada", notFound.Value.ToString().ToLower());
    }

    [Fact]
    public async Task GetOrdersDelivered_ReturnsOk_WhenFound()
    {
        var delivered = new List<OrderDelivery> { new OrderDelivery { Id = 2 } };

        _deliveryServiceMock
            .Setup(x => x.GetAll())
            .ReturnsAsync(delivered);

        var result = await _controller.GetOrdersDelivered();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(delivered, ok.Value);
    }


    [Fact]
    public async Task Create_ReturnsCreated_WhenValid()
    {
        var order = new Order { Id = 1 };
        var mockFile = new Mock<IFormFile>();
        var trasabilityId = "user123";

        _userServiceMock.Setup(x => x.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>())).Returns(trasabilityId);
        _orderServiceMock.Setup(x => x.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("filepath");
        _orderServiceMock.Setup(x => x.AddAsync(order, "filepath", trasabilityId)).Returns(Task.CompletedTask);

        var result = await _controller.Create(order, mockFile.Object);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(order, created.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        _orderServiceMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }


}
