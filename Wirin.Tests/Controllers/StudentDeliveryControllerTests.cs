using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wirin.Api.Controllers;
using Wirin.Api.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Xunit;

public class StudentDeliveryControllerTests
{
    private readonly Mock<IStudentDeliveryRepository> _studentDeliveryRepoMock;
    private readonly Mock<IOrderDeliveryRepository> _orderDeliveryRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly StudentDeliveryService _studentDeliveryService;
    private readonly UserService _userService;
    private readonly StudentDeliveryController _controller;

    public StudentDeliveryControllerTests()
    {
        _studentDeliveryRepoMock = new Mock<IStudentDeliveryRepository>();
        _orderDeliveryRepoMock = new Mock<IOrderDeliveryRepository>();
        _userRepoMock = new Mock<IUserRepository>();

        _studentDeliveryService = new StudentDeliveryService(
            _studentDeliveryRepoMock.Object,
            _orderDeliveryRepoMock.Object
        );

        _userService = new UserService(_userRepoMock.Object);

        _controller = new StudentDeliveryController(_studentDeliveryService, _userService);
    }



    [Fact]
    public async Task GetStudentDeliveriesAsync_ReturnsOk_WithList()
    {
        // Arrange
        var mockDeliveries = new List<StudentDelivery> { new StudentDelivery(), new StudentDelivery() };

        _studentDeliveryRepoMock
            .Setup(r => r.GetAllStudentDeliveryAsync())
            .ReturnsAsync(mockDeliveries);

        // Act
        var result = await _controller.GetStudentDeliveriesAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockDeliveries, okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenDeliveryCreated()
    {
        var request = new StudentDeliveryRequest
        {
            StudentId = "student123",
            OrderDeliveryId = 42,
            CreateDate = new DateTime(2025, 1, 1)
        };

        _orderDeliveryRepoMock
            .Setup(r => r.GetByIdAsync(request.OrderDeliveryId))
            .ReturnsAsync(new OrderDelivery
            {
                Id = request.OrderDeliveryId,
                Status = "Pendiente"
            });

        var result = await _controller.Create(request);

        _studentDeliveryRepoMock.Verify(r => r.AddStudentDeliveryAsync(It.Is<StudentDelivery>(d =>
            d.StudentId == request.StudentId &&
            d.OrderDeliveryId == request.OrderDeliveryId &&
            d.CreateDate == request.CreateDate
        )), Times.Once);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<OkObjectResult>(result).Value;
        var message = response.GetType().GetProperty("message")?.GetValue(response)?.ToString();
        Assert.Equal("Entrega del estudiante creada correctamente.", message);
    }

    [Fact]
    public async Task GetUsersWithoutOrderDelivery_ReturnsAllStudents_WhenOrderDeliveryIdIsZero()
    {
        var allStudents = new List<User>
{
    new User { Id = "1", UserName = "juan123", FullName = "Juan Pérez" },
    new User { Id = "2", UserName = "maria456", FullName = "Maria Lopez" }
};


        _userRepoMock.Setup(r => r.GetAllStudentsAsync()).ReturnsAsync(allStudents);

        var result = await _controller.GetUsersWithoutOrderDelivery(0);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudents = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Equal(allStudents.Count, ((List<User>)returnedStudents).Count);
    }

    [Fact]
    public async Task GetUsersWithoutOrderDelivery_FiltersStudents_WhenOrderDeliveryIdIsNotZero()
    {
            var allStudents = new List<User>
    {
        new User { Id = "1", UserName = "juan123", FullName = "Juan Pérez" },
        new User { Id = "2", UserName = "maria456", FullName = "Maria Lopez" }
    };


            var filteredStudents = new List<User>
    {
        new User { Id = "2", UserName = "maria456", FullName = "Maria Lopez" }
    };


        _userRepoMock.Setup(r => r.GetAllStudentsAsync()).ReturnsAsync(allStudents);
        _studentDeliveryRepoMock.Setup(r => r.GetAllStudentDeliveryAsync()).ReturnsAsync(new List<StudentDelivery>
            {
                new StudentDelivery { StudentId = "1", OrderDeliveryId = 5 }
            });

        var result = await _controller.GetUsersWithoutOrderDelivery(5);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStudents = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Single(returnedStudents);
        Assert.Contains(returnedStudents, u => u.Id == "2");
    }


}
