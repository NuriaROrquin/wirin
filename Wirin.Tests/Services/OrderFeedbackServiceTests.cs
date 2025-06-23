using Xunit;
using Moq;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Wirin.Tests.Services
{
    public class OrderFeedbackServiceTests
    {
        private readonly Mock<IOrderFeedbackRepository> _mockOrderFeedbackRepository;
        private readonly OrderFeedbackService _service;

        public OrderFeedbackServiceTests()
        {
            _mockOrderFeedbackRepository = new Mock<IOrderFeedbackRepository>();
            _service = new OrderFeedbackService(_mockOrderFeedbackRepository.Object);
        }

        [Fact]
        public async Task GetAllOrderFeedbacksAsync_ReturnsListOfOrderFeedbacks()
        {
            // Arrange
            var orderFeedbacks = new List<OrderFeedback>
            {
                new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 },
                new OrderFeedback { Id = 2, StudentId = "student2", FeedbackText = "Good!", Stars = 4, OrderId = 102 }
            };
            _mockOrderFeedbackRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(orderFeedbacks);

            // Act
            var result = await _service.GetAllOrderFeedbacksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrderFeedbackByIdAsync_ReturnsOrderFeedback_WhenFound()
        {
            // Arrange
            var orderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 };
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(orderFeedback);

            // Act
            var result = await _service.GetOrderFeedbackByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetOrderFeedbackByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((OrderFeedback)null);

            // Act
            var result = await _service.GetOrderFeedbackByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddOrderFeedbackAsync_ReturnsOrderFeedback_WhenValid()
        {
            // Arrange
            var newOrderFeedback = new OrderFeedback { StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 };
            var addedOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Great!", Stars = 5, OrderId = 101 };
            _mockOrderFeedbackRepository.Setup(r => r.AddAsync(It.IsAny<OrderFeedback>())).ReturnsAsync(addedOrderFeedback);

            // Act
            var result = await _service.AddOrderFeedbackAsync(newOrderFeedback);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockOrderFeedbackRepository.Verify(r => r.AddAsync(It.IsAny<OrderFeedback>()), Times.Once);
        }

        [Theory]
        [InlineData("", 5)] // Empty FeedbackText
        [InlineData("Test", 0)] // Stars out of range (too low)
        [InlineData("Test", 6)] // Stars out of range (too high)
        public async Task AddOrderFeedbackAsync_ReturnsNull_WhenInvalid(string feedbackText, int stars)
        {
            // Arrange
            var newOrderFeedback = new OrderFeedback { StudentId = "student1", FeedbackText = feedbackText, Stars = stars, OrderId = 101 };

            // Act
            var result = await _service.AddOrderFeedbackAsync(newOrderFeedback);

            // Assert
            Assert.Null(result);
            _mockOrderFeedbackRepository.Verify(r => r.AddAsync(It.IsAny<OrderFeedback>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOrderFeedbackAsync_ReturnsTrue_WhenFoundAndValid()
        {
            // Arrange
            var existingOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Old!", Stars = 3, OrderId = 101 };
            var updatedOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "New!", Stars = 4, OrderId = 101 };
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrderFeedback);
            _mockOrderFeedbackRepository.Setup(r => r.UpdateAsync(It.IsAny<OrderFeedback>())).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateOrderFeedbackAsync(1, updatedOrderFeedback);

            // Assert
            Assert.True(result);
            _mockOrderFeedbackRepository.Verify(r => r.UpdateAsync(It.IsAny<OrderFeedback>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderFeedbackAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            var updatedOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "New!", Stars = 4, OrderId = 101 };
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((OrderFeedback)null);

            // Act
            var result = await _service.UpdateOrderFeedbackAsync(1, updatedOrderFeedback);

            // Assert
            Assert.False(result);
            _mockOrderFeedbackRepository.Verify(r => r.UpdateAsync(It.IsAny<OrderFeedback>()), Times.Never);
        }

        [Theory]
        [InlineData("", 5)] // Empty FeedbackText
        [InlineData("Test", 0)] // Stars out of range (too low)
        [InlineData("Test", 6)] // Stars out of range (too high)
        public async Task UpdateOrderFeedbackAsync_ReturnsFalse_WhenInvalid(string feedbackText, int stars)
        {
            // Arrange
            var existingOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Old!", Stars = 3, OrderId = 101 };
            var updatedOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = feedbackText, Stars = stars, OrderId = 101 };
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrderFeedback);

            // Act
            var result = await _service.UpdateOrderFeedbackAsync(1, updatedOrderFeedback);

            // Assert
            Assert.False(result);
            _mockOrderFeedbackRepository.Verify(r => r.UpdateAsync(It.IsAny<OrderFeedback>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOrderFeedbackAndHandleNotFoundAsync_ReturnsTrue_WhenFound()
        {
            // Arrange
            var existingOrderFeedback = new OrderFeedback { Id = 1, StudentId = "student1", FeedbackText = "Old!", Stars = 3, OrderId = 101 };
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrderFeedback);
            _mockOrderFeedbackRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteOrderFeedbackAndHandleNotFoundAsync(1);

            // Assert
            Assert.True(result);
            _mockOrderFeedbackRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderFeedbackAndHandleNotFoundAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _mockOrderFeedbackRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((OrderFeedback)null);

            // Act
            var result = await _service.DeleteOrderFeedbackAndHandleNotFoundAsync(1);

            // Assert
            Assert.False(result);
            _mockOrderFeedbackRepository.Verify(r => r.DeleteAsync(1), Times.Never);
        }
    }
}