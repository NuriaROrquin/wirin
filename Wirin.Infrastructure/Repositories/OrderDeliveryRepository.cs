using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class OrderDeliveryRepository : IOrderDeliveryRepository
{
    private readonly WirinDbContext _context;

    public OrderDeliveryRepository(WirinDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDelivery>> GetAllByUserIdAsync(string studentUserId)
    {

        var orderDeliveries = await _context.OrderDeliveries
            .Where(od => od.UserId == studentUserId)
            .ToListAsync();

        var orderDeliveriesTransformed = orderDeliveries
            .Select(OrderDeliveryTransformer.ToDomain)
            .ToList();

        return orderDeliveriesTransformed;
    }

    public async Task<OrderDelivery> GetByIdAsync(int id)
    {

        var orderDeliveryEntity = await _context.OrderDeliveries
            .FirstOrDefaultAsync(od => od.Id == id);

        var orderDelivery = OrderDeliveryTransformer.ToDomain(orderDeliveryEntity);
        return orderDelivery;
    }

    public async Task AddAsync(OrderDelivery orderDelivery)
    {

        var orderDeliveryEntity = OrderDeliveryTransformer.ToEntity(orderDelivery);

        await _context.OrderDeliveries.AddAsync(orderDeliveryEntity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderDelivery orderDelivery)
    {
        var orderDeliveryEntity = OrderDeliveryTransformer.ToEntity(orderDelivery);

        _context.OrderDeliveries.Update(orderDeliveryEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var orderDelivery = await GetByIdAsync(id);
        if (orderDelivery != null)
        {
            var orderDeliveryEntity = OrderDeliveryTransformer.ToEntity(orderDelivery);

            _context.OrderDeliveries.Remove(orderDeliveryEntity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<OrderDelivery> AddNewDeliveryAsync(string studentId, string userId)
    {
        var od = new OrderDeliveryEntity
        {
            StudentUserId = studentId,
            UserId = userId,
            Status = "Entregada",
        };

        _context.OrderDeliveries.Add(od);
        await _context.SaveChangesAsync();

        var transformed = OrderDeliveryTransformer.ToDomain(od);

        return transformed;

    }

    public async Task<List<OrderDelivery>> GetAll()
    {
        var orderDeliveries = await _context.OrderDeliveries.ToListAsync();

        var orderDeliveriesTransformed = orderDeliveries
            .Select(OrderDeliveryTransformer.ToDomain)
            .ToList();

        return orderDeliveriesTransformed;
    }

    public async Task<List<OrderDelivery>> GetAllWithOrders()
    {
        try
        {
            var orderDeliveries = await _context.OrderDeliveries.ToListAsync();
            var orderSequences = await _context.OrderSequences.ToListAsync();
            var orders = await _context.Orders.ToListAsync();

            var orderDeliveriesTransformed = orderDeliveries
                .Select(orderDelivery =>
                {
                    var transformedOrderDelivery = OrderDeliveryTransformer.ToDomain(orderDelivery);
                    transformedOrderDelivery.Orders = orders
                        .Where(order => orderSequences.Any(seq => seq.OrderId == order.Id && seq.OrderDeliveryId == orderDelivery.Id))
                        .Select(OrderTransformer.ToDomain)
                        .ToList();

                    return transformedOrderDelivery;
                })
                .ToList();

            return orderDeliveriesTransformed;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los proyectos con sus órdenes.");
        }
    }

    //GetAllWithOrdersByStudentId
    public async Task<List<OrderDelivery>> GetAllWithOrdersByStudentId(string studentId)
    {
        try
        {
            var orderDeliveries = await _context.OrderDeliveries
                .Where(od => od.StudentUserId == studentId)
                .ToListAsync();

            var orderDeliveryIds = orderDeliveries.Select(od => od.Id).ToList();
            
            var orderSequences = await _context.OrderSequences
                .Where(os => orderDeliveryIds.Contains(os.OrderDeliveryId))
                .ToListAsync();
                
            var orderIds = orderSequences.Select(os => os.OrderId).ToList();
            
            var orders = await _context.Orders
                .Where(o => orderIds.Contains(o.Id))
                .ToListAsync();
                
            var paragraphs = await _context.Paragraph
                .Where(p => orderIds.Contains(p.OrderId))
                .ToListAsync();

            var orderDeliveriesTransformed = orderDeliveries
                .Select(orderDelivery =>
                {
                    var transformedOrderDelivery = OrderDeliveryTransformer.ToDomain(orderDelivery);
                    
                    var relatedOrders = orders
                        .Where(order => orderSequences.Any(seq => seq.OrderId == order.Id && seq.OrderDeliveryId == orderDelivery.Id))
                        .Select(OrderTransformer.ToDomain)
                        .ToList();
                    
                    transformedOrderDelivery.Orders = relatedOrders;
                    
                    // Agrupar los textos de párrafos por OrderId, ordenados por página y por secuencia de orden
                    var orderTextsGrouped = paragraphs
                        .Where(p => orders.Any(o => o.Id == p.OrderId))
                        .GroupBy(p => p.OrderId)
                        .ToDictionary(g => g.Key, g => g.OrderBy(p => p.PageNumber)
                                                        .ThenBy(p => orderSequences.FirstOrDefault(seq => seq.OrderId == p.OrderId)?.Order ?? 0)
                                                        .Select(p => p.ParagraphText)
                                                        .ToList());
                    
                    // Agregar los párrafos agrupados al OrderDelivery, agrupados por OrderId según OrderSequence y luego ordenados por página
                    transformedOrderDelivery.OrderParagraphs = paragraphs
                        .Where(p => relatedOrders.Any(o => o.Id == p.OrderId))
                        .OrderBy(p => orderSequences.FirstOrDefault(seq => seq.OrderId == p.OrderId)?.Order ?? 0)
                        .ThenBy(p => p.PageNumber)
                        .Select(ParagraphTransformer.ToDomain)
                        .ToList();

                    return transformedOrderDelivery;
                })
                .ToList();

            return orderDeliveriesTransformed;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los proyectos con sus órdenes.");
        }
    }

}
