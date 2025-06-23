using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class OrderSequenceRepository : IOrderSequenceRepository
{
    private readonly WirinDbContext _context;

    public OrderSequenceRepository(WirinDbContext context)
    {
        _context = context;
    }
    public async Task CreateSequenceAsync(List<OrderSequence> ordersSquences, int orderDeliveryId)
    {
        try
        {

            if (ordersSquences == null || !ordersSquences.Any())
            {
                throw new ArgumentException("Order sequences cannot be null or empty.", nameof(ordersSquences));
            }

            var items = new List<OrderSequenceEntity>();
               

            foreach(var item in ordersSquences)
            {
                var element = new OrderSequenceEntity
                {
                    Order = item.Order,
                    OrderDeliveryId = orderDeliveryId,
                    OrderId = item.OrderId
                };

                items.Add(element);
            }

            await _context.OrderSequences.AddRangeAsync(items);
            await _context.SaveChangesAsync();

        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }

    public async Task DeleteSequenceAsync(int orderSequenceId)
    {

        var orderSequence = await _context.OrderSequences.FindAsync(orderSequenceId);
        if (orderSequence == null)
        {
            throw new KeyNotFoundException($"Order sequence with ID {orderSequenceId} not found.");
        }

        _context.OrderSequences.Remove(orderSequence);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderSequence>> GetAllSequencesAsync()
    {
        var list = _context.OrderSequences.Select(OrderSequenceTransformer.ToDomain).ToList();
        return list;
    }

    public async Task PerformDelivery(List<OrderSequence> orderList, int orderDeliveryId)
    {

        try
        {
            await CreateSequenceAsync(orderList, orderDeliveryId);
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }

    public async Task<bool> SequenceExistsAsync(int orderSequenceId)
    {
        return await _context.OrderSequences.AnyAsync(os => os.Id == orderSequenceId);
    }

    public async Task UpdateSequenceAsync(OrderSequence orderSquence)
    {
        if (orderSquence == null)
        {
            throw new ArgumentNullException(nameof(orderSquence), "Order sequence cannot be null.");
        }

        var existingSequence = _context.OrderSequences.Find(orderSquence.Id);
        if (existingSequence == null)
        {
            throw new KeyNotFoundException($"Order sequence with ID {orderSquence.Id} not found.");
        }

        existingSequence.OrderId = orderSquence.OrderId;
        existingSequence.Order = orderSquence.Order;
        existingSequence.OrderDeliveryId = orderSquence.OrderDeliveryId;

       _context.OrderSequences.Update(existingSequence);
        await _context.SaveChangesAsync();
    }
}
