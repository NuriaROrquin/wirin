using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Repositories;
using Xunit;

public class StudentDeliveryRepositoryTests
{
    private WirinDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new WirinDbContext(options);
    }

    [Fact]
    public async Task AddStudentDeliveryAsync_AddsDeliveryToDatabase()
    {
        using var context = CreateContext(nameof(AddStudentDeliveryAsync_AddsDeliveryToDatabase));
        var repository = new StudentDeliveryRepository(context);

        var delivery = new StudentDelivery
        {
            StudentId = "student1",
            OrderDeliveryId = 1,
            CreateDate = DateTime.UtcNow
        };

        await repository.AddStudentDeliveryAsync(delivery);

        var saved = await context.StudentDeliveries.FirstOrDefaultAsync(sd =>
            sd.StudentId == "student1" && sd.OrderDeliveryId == 1);

        Assert.NotNull(saved);
        Assert.Equal("student1", saved.StudentId);
        Assert.Equal(1, saved.OrderDeliveryId);
    }

    [Fact]
    public async Task GetAllStudentDeliveryAsync_ReturnsAllDeliveries()
    {
        using var context = CreateContext(nameof(GetAllStudentDeliveryAsync_ReturnsAllDeliveries));
        context.StudentDeliveries.Add(new Wirin.Infrastructure.Entities.StudentDeliveryEntity
        {
            StudentId = "student1",
            OrderDeliveryId = 1,
            CreateDate = DateTime.UtcNow
        });
        context.StudentDeliveries.Add(new Wirin.Infrastructure.Entities.StudentDeliveryEntity
        {
            StudentId = "student2",
            OrderDeliveryId = 2,
            CreateDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var repository = new StudentDeliveryRepository(context);

        var deliveries = await repository.GetAllStudentDeliveryAsync();

        Assert.Equal(2, deliveries.Count());
        Assert.Contains(deliveries, d => d.StudentId == "student1" && d.OrderDeliveryId == 1);
        Assert.Contains(deliveries, d => d.StudentId == "student2" && d.OrderDeliveryId == 2);
    }

    [Fact]
    public async Task GetStudentDeliveriesAsync_ReturnsAllDeliveriesTransformed()
    {
        using var context = CreateContext(nameof(GetStudentDeliveriesAsync_ReturnsAllDeliveriesTransformed));
        context.StudentDeliveries.AddRange(
            new Wirin.Infrastructure.Entities.StudentDeliveryEntity
            {
                StudentId = "student1",
                OrderDeliveryId = 1,
                CreateDate = DateTime.UtcNow
            },
            new Wirin.Infrastructure.Entities.StudentDeliveryEntity
            {
                StudentId = "student2",
                OrderDeliveryId = 2,
                CreateDate = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();

        var repository = new StudentDeliveryRepository(context);

        var result = await repository.GetStudentDeliveriesAsync();
        var resultList = result.ToList();

        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, r => r.StudentId == "student1" && r.OrderDeliveryId == 1);
        Assert.Contains(resultList, r => r.StudentId == "student2" && r.OrderDeliveryId == 2);
    }

    [Fact]
    public async Task AddStudentDeliveryAsync_AllowsDuplicatesUnlessRestricted()
    {
        using var context = CreateContext(nameof(AddStudentDeliveryAsync_AllowsDuplicatesUnlessRestricted));
        var repository = new StudentDeliveryRepository(context);

        var delivery1 = new StudentDelivery { StudentId = "student1", OrderDeliveryId = 1, CreateDate = DateTime.UtcNow };
        var delivery2 = new StudentDelivery { StudentId = "student1", OrderDeliveryId = 1, CreateDate = DateTime.UtcNow };

        await repository.AddStudentDeliveryAsync(delivery1);
        await repository.AddStudentDeliveryAsync(delivery2);

        var count = await context.StudentDeliveries.CountAsync(sd =>
            sd.StudentId == "student1" && sd.OrderDeliveryId == 1);

        Assert.Equal(2, count); // Cambiar a 1 si la lógica debe impedir duplicados
    }
}