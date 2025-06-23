using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Wirin.Api.Controllers;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Wirin.Domain.Repositories;

namespace Wirin.Tests.Controllers
{
    public class OrderFeedbackControllerTests
    {
        private readonly Mock<OrderFeedbackService> _mockOrderFeedbackService;
        private readonly OrderFeedbackController _controller;

        public OrderFeedbackControllerTests()
        {
            var _mockOrderFeedbackRepository = new Mock<IOrderFeedbackRepository>();
            _mockOrderFeedbackService = new Mock<OrderFeedbackService>(_mockOrderFeedbackRepository.Object);
            _controller = new OrderFeedbackController(_mockOrderFeedbackService.Object);
        }

        [Fact]
        public async Task GetAllOrderFeedbacks_ReturnsOkResult_WithListOfOrderFeedbacks()
        {
            // Arrange
            var orderFeedbacks = new List<OrderFeedback>
            {
                new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 },
                new OrderFeedback { Id = 2, StudentId = "student2", FeedbackText = "Good!", Stars = 4, OrderId = 102 }
            };
            _mockOrderFeedbackService.Setup(s => s.GetAllOrderFeedbacksAsync()).ReturnsAsync(orderFeedbacks);

            // Act
            var result = await _controller.GetAllOrderFeedbacks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<OrderFeedbackResponse>>(okResult.Value);

            Assert.Equal(2, returnValue.Count());

        }

        [Fact]
        public async Task GetOrderFeedbackById_ReturnsOkResult_WithOrderFeedback()
        {
            // Arrange
            var orderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 };
            _mockOrderFeedbackService.Setup(s => s.GetOrderFeedbackByIdAsync(1)).ReturnsAsync(orderFeedback);

            // Act
            var result = await _controller.GetOrderFeedbackById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderFeedbackResponse>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetOrderFeedbackById_ReturnsNotFound_WhenOrderFeedbackDoesNotExist()
        {
            // Arrange
            _mockOrderFeedbackService.Setup(s => s.GetOrderFeedbackByIdAsync(1)).ReturnsAsync((OrderFeedback)null);

            // Act
            var result = await _controller.GetOrderFeedbackById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateOrderFeedback_ReturnsCreatedAtActionResult_WithOrderFeedback()
        {
            // Arrange
            var request = new CreateOrderFeedbackRequest { StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 };
            var orderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 };
            _mockOrderFeedbackService
                .Setup(s => s.AddOrderFeedbackAsync(It.IsAny<OrderFeedback>()))
                .ReturnsAsync(orderFeedback);


            // Act
            var result = await _controller.CreateOrderFeedback(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<OrderFeedbackResponse>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task CreateOrderFeedback_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new CreateOrderFeedbackRequest { StudentId = "student1", FeedbackText = "", Stars = 0, OrderId = 101 }; // Invalid data
            _controller.ModelState.AddModelError("FeedbackText", "FeedbackText is required");

            // Act
            var result = await _controller.CreateOrderFeedback(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateOrderFeedback_ReturnsNoContentResult()
        {
            // Arrange
            var request = new UpdateOrderFeedbackRequest { StudentId = "student1", FeedbackText = "Updated!", Stars = 4, OrderId = 101 };
            _mockOrderFeedbackService.Setup(s => s.UpdateOrderFeedbackAsync(1, It.IsAny<OrderFeedback>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrderFeedback(1, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateOrderFeedback_ReturnsNotFound_WhenOrderFeedbackDoesNotExist()
        {
            // Arrange
            var request = new UpdateOrderFeedbackRequest { StudentId = "student1", FeedbackText = "Updated!", Stars = 4, OrderId = 101 };
            _mockOrderFeedbackService.Setup(s => s.UpdateOrderFeedbackAsync(1, It.IsAny<OrderFeedback>())).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateOrderFeedback(1, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateOrderFeedback_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new UpdateOrderFeedbackRequest { StudentId = "student1", FeedbackText = "", Stars = 0, OrderId = 101 }; // Invalid data
            _controller.ModelState.AddModelError("FeedbackText", "FeedbackText is required");

            // Act
            var result = await _controller.UpdateOrderFeedback(1, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteOrderFeedback_ReturnsNoContentResult()
        {
            // Arrange
            _mockOrderFeedbackService.Setup(s => s.DeleteOrderFeedbackAndHandleNotFoundAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrderFeedback(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrderFeedback_ReturnsNotFound_WhenOrderFeedbackDoesNotExist()
        {
            // Arrange
            _mockOrderFeedbackService.Setup(s => s.DeleteOrderFeedbackAndHandleNotFoundAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteOrderFeedback(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}