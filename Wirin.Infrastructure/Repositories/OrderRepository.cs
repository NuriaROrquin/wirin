using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly WirinDbContext _context;

    public OrderRepository(WirinDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        // Devuelve todas las tareas como una lista
        var orders = await _context.Orders.ToListAsync();
        // Convierte las entidades de la base de datos a entidades de dominio
        var orderList = orders.Select(OrderTransformer.ToDomain).ToList();

        return orderList;
    }

    public async Task<Order> GetByIdAsync(int OrderId)
    {
        // Busca una OrderInDB específica por su ID
        var order = await _context.Orders.FindAsync(OrderId);
        // Convierte la entidad de la base de datos a una entidad de dominio
        var orderDomain = OrderTransformer.ToDomain(order);

        return orderDomain;
    }


    public async Task AddAsync(Order order)
    {
        var orderEntity = OrderTransformer.ToEntity(order);
        await _context.Orders.AddAsync(orderEntity);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(Order order)
    {
        var orderEntity = await _context.Orders.FindAsync(order.Id);
      
        if(orderEntity == null)
        {
            throw new Exception("Tarea no encontrada.");
        }
            
        _context.Entry(orderEntity).CurrentValues.SetValues(order);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int OrderId)
    {
        // Busca y elimina una OrderInDB
        var orderEntity = await _context.Orders.FindAsync(OrderId);
        if (orderEntity != null)
        {

            _context.Orders.Remove(orderEntity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Tarea no encontrada.");
        }
    }
}
