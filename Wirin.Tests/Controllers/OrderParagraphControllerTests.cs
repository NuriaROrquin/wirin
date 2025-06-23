using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wirin.Api.Controllers;
using Wirin.Domain.Services;
using Wirin.Infrastructure.Services;
using Wirin.Infrastructure.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Repositories;
using Wirin.Domain.Dtos.Request;

public class OrderParagraphControllerTests
{
    private readonly Mock<OrderParagraphService> _paragraphServiceMock;
    private readonly Mock<UserService> _userServiceMock;
    private readonly OrderParagraphController _controller;

    public OrderParagraphControllerTests()
    {
        var mockOrderParagraphRepo = new Mock<IOrderParagraphRepository>();
        var mockOrderTrasabilitiRepo = new Mock<IOrderTrasabilityRepository>();
        var mockOrderAnnotationsRepo = new Mock<IParagraphAnnotationRepository>();
        var mockUserRepo = new Mock<IUserRepository>();

        _paragraphServiceMock = new Mock<OrderParagraphService>(mockOrderParagraphRepo.Object, mockOrderAnnotationsRepo.Object, mockOrderTrasabilitiRepo.Object);
        _userServiceMock = new Mock<UserService>(mockUserRepo.Object);

        _controller = new OrderParagraphController(_paragraphServiceMock.Object, _userServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task ProcessOrdersAsync_ReturnsOk_WhenRequestIsValid()
    {
        var request = new SaveParagraphRequest();

        _userServiceMock.Setup(s => s.GetUserTrasabilityId(It.IsAny<ClaimsPrincipal>()))
            .Returns("user123");

        _paragraphServiceMock
            .Setup(s => s.UpdateParagraphAsync(request, "user123", "user123"))
            .Returns(Task.CompletedTask);

        var result = await _controller.ProcessOrdersAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Orders processed successfully.", ok.Value);
    }

    [Fact]
    public async Task ProcessOrdersAsync_ReturnsBadRequest_WhenRequestIsNull()
    {
        var result = await _controller.ProcessOrdersAsync(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No paragraph to process.", badRequest.Value);
    }

    [Fact]
    public async Task GetParagraphsByOrderIdAsync_ReturnsOk_WhenFound()
    {
        var paragraphsResponse = new List<OrderParagraphResponse>
    {
        new OrderParagraphResponse { OrderId = 1, PageNumber = 1, ParagraphText = "Example" }
    };

        _paragraphServiceMock
            .Setup(s => s.GetParagraphsByOrderIdAsync(1))
            .ReturnsAsync(paragraphsResponse);

        var result = await _controller.GetParagraphsByOrderIdAsync(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paragraphsResponse, ok.Value);
    }

    [Fact]
    public async Task GetParagraphsByOrderIdAsync_ReturnsNotFound_WhenEmpty()
    {
        _paragraphServiceMock
            .Setup(s => s.GetParagraphsByOrderIdAsync(1))
            .ReturnsAsync(new List<OrderParagraphResponse>());

        var result = await _controller.GetParagraphsByOrderIdAsync(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("no paragraphs", notFound.Value.ToString().ToLower());
    }
}
