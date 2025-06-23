using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wirin.Api.Dtos.Response;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Repositories;
using Xunit;

public class OrderTrasabilityRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WirinDbContext(options);
    }

    [Fact]
    public async Task SaveAsync_AddsNewOrderTrasability()
    {
        var context = GetDbContext();
        var repo = new OrderTrasabilityRepository(context);

        var orderTrasability = new OrderTrasability
        {
            OrderId = 1,
            Action = "Created",
            UserId = "user1",
            ProcessedAt = DateTime.UtcNow
        };

        await repo.SaveAsync(orderTrasability);

        var all = context.OrderTrasability.ToList();
        Assert.Single(all);
        Assert.Equal("Created", all[0].Action);
    }

    [Fact]
    public async Task GetAllOrderTrasabilities_ReturnsAllItems()
    {
        var context = GetDbContext();
        context.OrderTrasability.AddRange(new List<OrderTrasabilityEntity>
        {
            new OrderTrasabilityEntity { OrderId = 1, Action = "A1", UserId = "u1", ProcessedAt = DateTime.UtcNow },
            new OrderTrasabilityEntity { OrderId = 2, Action = "A2", UserId = "u2", ProcessedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repo = new OrderTrasabilityRepository(context);

        var result = await repo.GetAllOrderTrasabilities();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByActionAsync_FiltersCorrectly()
    {
        var context = GetDbContext();
        context.OrderTrasability.AddRange(new List<OrderTrasabilityEntity>
        {
            new OrderTrasabilityEntity { OrderId = 1, Action = "Action1", UserId = "u1", ProcessedAt = DateTime.UtcNow },
            new OrderTrasabilityEntity { OrderId = 2, Action = "Action2", UserId = "u2", ProcessedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repo = new OrderTrasabilityRepository(context);

        var filtered = await repo.GetOrderTrasabilitiesByActionAsync("Action1");

        Assert.Single(filtered);
        Assert.All(filtered, o => Assert.Equal("Action1", o.Action));
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByOrderIdAsync_FiltersCorrectly()
    {
        var context = GetDbContext();
        context.OrderTrasability.AddRange(new List<OrderTrasabilityEntity>
        {
            new OrderTrasabilityEntity { OrderId = 5, Action = "X", UserId = "u1", ProcessedAt = DateTime.UtcNow },
            new OrderTrasabilityEntity { OrderId = 6, Action = "Y", UserId = "u2", ProcessedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repo = new OrderTrasabilityRepository(context);

        var filtered = await repo.GetOrderTrasabilitiesByOrderIdAsync(5);

        Assert.Single(filtered);
        Assert.All(filtered, o => Assert.Equal(5, o.OrderId));
    }

    [Fact]
    public async Task GetOrderTrasabilitiesByUserAsync_FiltersCorrectly()
    {
        var context = GetDbContext();
        context.OrderTrasability.AddRange(new List<OrderTrasabilityEntity>
        {
            new OrderTrasabilityEntity { OrderId = 10, Action = "A", UserId = "UserX", ProcessedAt = DateTime.UtcNow },
            new OrderTrasabilityEntity { OrderId = 11, Action = "B", UserId = "UserY", ProcessedAt = DateTime.UtcNow }
        });
        await context.SaveChangesAsync();

        var repo = new OrderTrasabilityRepository(context);

        var filtered = await repo.GetOrderTrasabilitiesByUserAsync("UserX");

        Assert.Single(filtered);
        Assert.All(filtered, o => Assert.Equal("UserX", o.UserId));
    }
}
