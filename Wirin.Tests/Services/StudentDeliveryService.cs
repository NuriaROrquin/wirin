using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Xunit;

public class StudentDeliveryServiceTests
{
    private readonly Mock<IStudentDeliveryRepository> _studentDeliveryRepositoryMock;
    private readonly StudentDeliveryService _sut;
    private readonly Mock<IOrderDeliveryRepository> _orderDeliveryRepositoryMock;

    public StudentDeliveryServiceTests()
    {
        _studentDeliveryRepositoryMock = new Mock<IStudentDeliveryRepository>();
        _orderDeliveryRepositoryMock = new Mock<IOrderDeliveryRepository>();

        _sut = new StudentDeliveryService(
            _studentDeliveryRepositoryMock.Object,
            _orderDeliveryRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CreateStudentDelivery_UpdatesOrderDeliveryStatus_WhenStatusIsCompletada()
    {
        var delivery = new StudentDelivery
        {
            StudentId = "student1",
            OrderDeliveryId = 10
        };

        var orderDelivery = new OrderDelivery
        {
            Id = 10,
            Status = "Completada"
        };

        _orderDeliveryRepositoryMock
            .Setup(r => r.GetByIdAsync(delivery.OrderDeliveryId))
            .ReturnsAsync(orderDelivery);

        await _sut.CreateStudentDelivery(delivery);

        _orderDeliveryRepositoryMock.Verify(r =>
            r.UpdateAsync(It.Is<OrderDelivery>(o => o.Status == "Entregada")), Times.Once);

        _studentDeliveryRepositoryMock.Verify(r =>
            r.AddStudentDeliveryAsync(delivery), Times.Once);
    }

    [Fact]
    public async Task GetUsersWithoutOrderDelivery_ReturnsFilteredStudents()
    {
        var orderDeliveryId = 10;
        var students = new List<User>
        {
            new User { Id = "s1" },
            new User { Id = "s2" },
            new User { Id = "s3" }
        };

        var deliveries = new List<StudentDelivery>
        {
            new StudentDelivery { StudentId = "s1", OrderDeliveryId = 10 },
            new StudentDelivery { StudentId = "s2", OrderDeliveryId = 5 }
        };

        _studentDeliveryRepositoryMock
            .Setup(r => r.GetAllStudentDeliveryAsync())
            .ReturnsAsync(deliveries);

        var result = await _sut.GetUsersWithoutOrderDelivery(students, orderDeliveryId);

        // Esperamos que sólo "s2" y "s3" estén porque "s1" ya tiene entrega para orderDeliveryId=10
        var resultList = result.ToList();

        Assert.DoesNotContain(resultList, u => u.Id == "s1");
        Assert.Contains(resultList, u => u.Id == "s2");
        Assert.Contains(resultList, u => u.Id == "s3");
    }
}
