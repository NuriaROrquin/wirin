using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class OrderManagmentRepository : IOrderManagmentRepository
{
    private readonly WirinDbContext _context;

    public OrderManagmentRepository(WirinDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllOrderByStatus(string state)
    {
        var orders = await _context.Orders.Where(t => t.Status == state).ToListAsync();

        var ordersDomain = orders.Select(OrderTransformer.ToDomain).ToList();

        return ordersDomain;
    }


    public async Task<List<Order>> GetAllByUserAssigned(string voluntarioId)
    {
        //obtener tareas por idUsuarioAsignado.
        var orders = await _context.Orders.Where(t => t.VoluntarioId == voluntarioId).ToListAsync();

        var ordersDomain = orders.Select(OrderTransformer.ToDomain).ToList();

        return ordersDomain;

    }

    public async Task UpdateAsync(Order order)
    {
        var existingEntity = await _context.Orders.FindAsync(order.Id);

        if (existingEntity == null)
            throw new Exception("Tarea no encontrada.");

        // Convertir el dominio a entidad para SetValues
        var updatedEntity = OrderTransformer.ToEntity(order);

        // Actualizar los valores actuales de la entidad existente con los del objeto actualizado
        _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);

        await _context.SaveChangesAsync();
    }


    public async Task<List<Order>> GetAllByStudentAsync(string studentUserId)
    {
        return await _context.OrderSequences
            .Join(_context.OrderDeliveries,
                os => os.OrderDeliveryId,
                od => od.Id,
                (os, od) => new { os, od })
            .Where(joined => joined.od.StudentUserId == studentUserId)
            .Join(_context.Orders,
                joined => joined.os.OrderId,
                o => o.Id,
                (joined, o) => new Order
                {
                    Id = o.Id,
                    Name = o.Name,
                    Subject = o.Subject,
                    Description = o.Description,
                    AuthorName = o.AuthorName,
                    rangePage = o.rangePage,
                })
            .AsNoTracking()
            .ToListAsync();
    }
}
