using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Repositories;
using Xunit;

public class OrderRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WirinDbContext(options);
    }

    private OrderEntity CreateOrderEntity(int id, string name = "Test Order") =>
        new OrderEntity
        {
            Id = id,
            Name = name,
            Subject = "Test Subject",
            Description = "Test Description",
            AuthorName = "Test Author",
            rangePage = "1-10",
            IsPriority = false,
            Status = "Active",
            CreationDate = DateTime.UtcNow
        };

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        context.Orders.AddRange(new List<OrderEntity>
        {
            CreateOrderEntity(1, "Order 1"),
            CreateOrderEntity(2, "Order 2")
        });
        await context.SaveChangesAsync();

        var orders = (await repo.GetAllAsync()).ToList();

        Assert.Equal(2, orders.Count);
        Assert.Contains(orders, o => o.Id == 1 && o.Name == "Order 1");
        Assert.Contains(orders, o => o.Id == 2 && o.Name == "Order 2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOrder_WhenFound()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        context.Orders.Add(CreateOrderEntity(1, "Order 1"));
        await context.SaveChangesAsync();

        var order = await repo.GetByIdAsync(1);

        Assert.NotNull(order);
        Assert.Equal(1, order.Id);
        Assert.Equal("Order 1", order.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        var order = await repo.GetByIdAsync(999);

        Assert.Null(order);
    }

    [Fact]
    public async Task AddAsync_AddsNewOrder()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        var newOrder = new Order
        {
            Id = 1,
            Name = "Orden de prueba",
            Subject = "Asunto de prueba",
            Description = "Descripción de prueba",
            AuthorName = "Autor de prueba",
            rangePage = "1-5",
            IsPriority = false,
            Status = "Pendiente",
            CreationDate = DateTime.UtcNow
        };

        await repo.AddAsync(newOrder);

        var orderInDb = await context.Orders.FirstOrDefaultAsync(o => o.Id == 1);

        Assert.NotNull(orderInDb);
        Assert.Equal("Orden de prueba", orderInDb.Name);
        Assert.Equal("Asunto de prueba", orderInDb.Subject);
        Assert.Equal("Pendiente", orderInDb.Status);
    }


    [Fact]
    public async Task UpdateAsync_UpdatesExistingOrder()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        var existingOrder = CreateOrderEntity(1, "Old Name");
        context.Orders.Add(existingOrder);
        await context.SaveChangesAsync();

        var updatedOrder = new Order { Id = 1, Name = "Updated Name" /* completar si hace falta */ };

        await repo.UpdateAsync(updatedOrder);

        var orderInDb = await context.Orders.FindAsync(1);
        Assert.NotNull(orderInDb);
        Assert.Equal("Updated Name", orderInDb.Name);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenOrderNotFound()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        var updatedOrder = new Order { Id = 999, Name = "No Order" };

        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await repo.UpdateAsync(updatedOrder);
        });
    }

    [Fact]
    public async Task DeleteAsync_DeletesOrder_WhenFound()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        var orderToDelete = CreateOrderEntity(1, "To Delete");
        context.Orders.Add(orderToDelete);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(1);

        var orderInDb = await context.Orders.FindAsync(1);
        Assert.Null(orderInDb);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenOrderNotFound()
    {
        var context = GetDbContext();
        var repo = new OrderRepository(context);

        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await repo.DeleteAsync(999);
        });
    }
}
