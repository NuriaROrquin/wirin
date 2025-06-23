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

public class OrderSequenceRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WirinDbContext(options);
    }

    [Fact]
    public async Task CreateSequenceAsync_AddsSequencesToDatabase()
    {
        var context = GetDbContext();
        var repo = new OrderSequenceRepository(context);

        var sequences = new List<OrderSequence>
        {
            new OrderSequence { OrderId = 1, Order = 1, OrderDeliveryId = 0 },
            new OrderSequence { OrderId = 2, Order = 2, OrderDeliveryId = 0 }
        };

        await repo.CreateSequenceAsync(sequences, orderDeliveryId: 5);

        var saved = await context.OrderSequences.ToListAsync();

        Assert.Equal(2, saved.Count);
        Assert.All(saved, s => Assert.Equal(5, s.OrderDeliveryId));
    }

    [Fact]
    public async Task CreateSequenceAsync_ThrowsArgumentException_WhenEmptyList()
    {
        var context = GetDbContext();
        var repo = new OrderSequenceRepository(context);

        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await repo.CreateSequenceAsync(new List<OrderSequence>(), 1);
        });

    }

    [Fact]
    public async Task DeleteSequenceAsync_RemovesSequence()
    {
        var context = GetDbContext();
        var entity = new OrderSequenceEntity { Id = 1, OrderId = 1, Order = 1, OrderDeliveryId = 1 };
        context.OrderSequences.Add(entity);
        await context.SaveChangesAsync();

        var repo = new OrderSequenceRepository(context);

        await repo.DeleteSequenceAsync(1);

        var deleted = await context.OrderSequences.FindAsync(1);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteSequenceAsync_ThrowsKeyNotFoundException_WhenNotFound()
    {
        var context = GetDbContext();
        var repo = new OrderSequenceRepository(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await repo.DeleteSequenceAsync(999);
        });
    }

    [Fact]
    public async Task GetAllSequencesAsync_ReturnsAllSequences()
    {
        var context = GetDbContext();
        context.OrderSequences.AddRange(new List<OrderSequenceEntity>
        {
            new OrderSequenceEntity { Id = 1, OrderId = 1, Order = 1, OrderDeliveryId = 1 },
            new OrderSequenceEntity { Id = 2, OrderId = 2, Order = 2, OrderDeliveryId = 2 }
        });
        await context.SaveChangesAsync();

        var repo = new OrderSequenceRepository(context);

        var sequences = await repo.GetAllSequencesAsync();

        Assert.Equal(2, sequences.Count);
    }

    [Fact]
    public async Task SequenceExistsAsync_ReturnsTrueIfExists()
    {
        var context = GetDbContext();
        context.OrderSequences.Add(new OrderSequenceEntity { Id = 1, OrderId = 1, Order = 1, OrderDeliveryId = 1 });
        await context.SaveChangesAsync();

        var repo = new OrderSequenceRepository(context);

        var exists = await repo.SequenceExistsAsync(1);

        Assert.True(exists);
    }

    [Fact]
    public async Task SequenceExistsAsync_ReturnsFalseIfNotExists()
    {
        var context = GetDbContext();
        var repo = new OrderSequenceRepository(context);

        var exists = await repo.SequenceExistsAsync(999);

        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateSequenceAsync_UpdatesExistingSequence()
    {
        var context = GetDbContext();
        var entity = new OrderSequenceEntity { Id = 1, OrderId = 1, Order = 1, OrderDeliveryId = 1 };
        context.OrderSequences.Add(entity);
        await context.SaveChangesAsync();

        var repo = new OrderSequenceRepository(context);

        var updatedSequence = new OrderSequence
        {
            Id = 1,
            OrderId = 10,
            Order = 20,
            OrderDeliveryId = 30
        };

        await repo.UpdateSequenceAsync(updatedSequence);

        var updatedEntity = await context.OrderSequences.FindAsync(1);

        Assert.Equal(10, updatedEntity.OrderId);
        Assert.Equal(20, updatedEntity.Order);
        Assert.Equal(30, updatedEntity.OrderDeliveryId);
    }

    [Fact]
    public async Task UpdateSequenceAsync_ThrowsArgumentNullException_WhenNull()
    {
        var context = GetDbContext();
        var repo = new OrderSequenceRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await repo.UpdateSequenceAsync(null);
        });
    }

    [Fact]
    public async Task UpdateSequenceAsync_ThrowsKeyNotFoundException_WhenNotFound()
    {
        var context = GetDbContext();
        var repo = new OrderSequenceRepository(context);

        var sequence = new OrderSequence { Id = 999, OrderId = 1, Order = 1, OrderDeliveryId = 1 };

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await repo.UpdateSequenceAsync(sequence);
        });
    }
}
