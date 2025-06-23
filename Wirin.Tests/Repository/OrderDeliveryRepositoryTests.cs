using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Repositories;

public class OrderDeliveryRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WirinDbContext(options);
    }

    [Fact]
    public async Task AddAsync_SavesOrderDelivery()
    {
        var context = GetDbContext();
        var repo = new OrderDeliveryRepository(context);

        var delivery = new OrderDelivery
        {
            Id = 1,
            StudentId = "stu1",
            UserId = "u1",
            Status = "En camino",
            Title = "Entrega 1",
            DeliveryDate = DateTime.UtcNow
        };

        await repo.AddAsync(delivery);

        var saved = await context.OrderDeliveries.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Entrega 1", saved.Title);
    }

    [Fact]
    public async Task GetAllWithOrdersByStudentId_ReturnsCorrectly()
    {
        var context = GetDbContext();
        var repo = new OrderDeliveryRepository(context);

        var delivery = new OrderDeliveryEntity { Id = 1, StudentUserId = "stu1", UserId = "u1", Status = "Entregado", Title = "Entrega A" };
        var order = new OrderEntity { Id = 10, Name = "Orden A", Subject = "Matemática", Description = "", AuthorName = "", rangePage = "", IsPriority = false, Status = "Entregado", CreationDate = DateTime.UtcNow };
        var seq = new OrderSequenceEntity { Id = 1, OrderId = 10, OrderDeliveryId = 1 };

        context.OrderDeliveries.Add(delivery);
        context.Orders.Add(order);
        context.OrderSequences.Add(seq);
        await context.SaveChangesAsync();

        var result = await repo.GetAllWithOrdersByStudentId("stu1");

        Assert.Single(result);
        Assert.Equal("Entrega A", result[0].Title);
        Assert.Single(result[0].Orders);
        Assert.Equal("Orden A", result[0].Orders[0].Name);
    }
}
