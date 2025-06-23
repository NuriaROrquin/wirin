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

public class OrderManagmentRepositoryTests
{
    private WirinDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WirinDbContext(options);
    }

    [Fact]
    public async Task GetAllOrderByStatus_ReturnsCorrectOrders()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new WirinDbContext(options);
        var repo = new OrderManagmentRepository(context);

        context.Orders.AddRange(
            new OrderEntity
            {
                Id = 1,
                Name = "T1",
                Status = "Pendiente",
                CreationDate = DateTime.Now,
                AuthorName = "Autor1",
                Description = "Descripción 1",
                Subject = "Materia 1",
                rangePage = "1-5",
                IsPriority = false
            },
            new OrderEntity
            {
                Id = 2,
                Name = "T2",
                Status = "Completado",
                CreationDate = DateTime.Now,
                AuthorName = "Autor2",
                Description = "Descripción 2",
                Subject = "Materia 2",
                rangePage = "2-4",
                IsPriority = false
            },
            new OrderEntity
            {
                Id = 3,
                Name = "T3",
                Status = "Pendiente",
                CreationDate = DateTime.Now,
                AuthorName = "Autor3",
                Description = "Descripción 3",
                Subject = "Materia 3",
                rangePage = "3-6",
                IsPriority = false
            }
        );
        await context.SaveChangesAsync();

        var result = await repo.GetAllOrderByStatus("Pendiente");

        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal("Pendiente", o.Status));
    }

    [Fact]
    public async Task GetAllByUserAssigned_ReturnsOrdersAssignedToUser()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new WirinDbContext(options);
        var repo = new OrderManagmentRepository(context);

        context.Orders.AddRange(
            new OrderEntity
            {
                Id = 1,
                Name = "O1",
                VoluntarioId = "v1",
                Status = "Pendiente",      // <--- Agregado
                CreationDate = DateTime.Now,
                AuthorName = "Autor1",
                Description = "Desc1",
                Subject = "Materia1",
                rangePage = "1-3",
                IsPriority = false
            },
            new OrderEntity
            {
                Id = 2,
                Name = "O2",
                VoluntarioId = "v2",
                Status = "Pendiente",      // <--- Agregado
                CreationDate = DateTime.Now,
                AuthorName = "Autor2",
                Description = "Desc2",
                Subject = "Materia2",
                rangePage = "2-4",
                IsPriority = false
            }
        );
        await context.SaveChangesAsync();

        var result = await repo.GetAllByUserAssigned("v1");

        Assert.Single(result);
        Assert.Equal("v1", result[0].VoluntarioId);
    }


    [Fact]
    public async Task UpdateAsync_UpdatesOrderCorrectly()
    {
        var context = GetDbContext();
        var repo = new OrderManagmentRepository(context);

        var existing = new OrderEntity
        {
            Id = 1,
            Name = "Original",
            Status = "Pendiente",
            CreationDate = DateTime.Now,
            AuthorName = "Autor original",
            Description = "Descripción original",
            Subject = "Materia original",
            rangePage = "1-2",
            IsPriority = false
        };

        context.Orders.Add(existing);
        await context.SaveChangesAsync();

        var updatedOrder = new Order
        {
            Id = 1,
            Name = "Modificado",
            Status = "Completado",
            CreationDate = existing.CreationDate,
            AuthorName = "Autor",
            Description = "Descripción actualizada",
            Subject = "Materia",
            rangePage = "1-3",
            IsPriority = false
        };

        await repo.UpdateAsync(updatedOrder);

        var dbOrder = await context.Orders.FindAsync(1);
        Assert.Equal("Modificado", dbOrder.Name);
        Assert.Equal("Completado", dbOrder.Status);
    }

    [Fact]
    public async Task GetAllByStudentAsync_ReturnsStudentOrders()
    {
        var options = new DbContextOptionsBuilder<WirinDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new WirinDbContext(options);
        var repo = new OrderManagmentRepository(context);

        var order = new OrderEntity
        {
            Id = 10,
            Name = "Orden A",
            Subject = "Historia",
            Description = "desc",
            AuthorName = "autor",
            rangePage = "1-5",
            Status = "Pendiente",  
            CreationDate = DateTime.Now
        };
        var delivery = new OrderDeliveryEntity
        {
            Id = 1,
            StudentUserId = "stu123",
            Status = "Pendiente",
            Title = "Entrega 1",
            UserId = "user1"
        };

        var sequence = new OrderSequenceEntity { Id = 1, OrderId = 10, OrderDeliveryId = 1 };

        context.Orders.Add(order);
        context.OrderDeliveries.Add(delivery);
        context.OrderSequences.Add(sequence);
        await context.SaveChangesAsync();

        var result = await repo.GetAllByStudentAsync("stu123");

        Assert.Single(result);
        Assert.Equal("Orden A", result[0].Name);
    }

}
