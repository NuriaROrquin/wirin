using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class OrderFeedbackRepository : IOrderFeedbackRepository
{
    private readonly WirinDbContext _context;

    public OrderFeedbackRepository(WirinDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderFeedback>> GetAllAsync()
    {
        var entities = await _context.OrderFeedbacks.ToListAsync();
        return OrderFeedbackTransformer.ToDomainList(entities);
    }

    public async Task<OrderFeedback> GetByIdAsync(int id)
    {
        var entity = await _context.OrderFeedbacks.FindAsync(id);
        return OrderFeedbackTransformer.ToDomain(entity);
    }

    public async Task<OrderFeedback> AddAsync(OrderFeedback orderFeedback)
    {
        var entity = OrderFeedbackTransformer.ToEntity(orderFeedback);
        await _context.OrderFeedbacks.AddAsync(entity);
        await _context.SaveChangesAsync();
        orderFeedback.Id = entity.Id; // Update the ID after saving
        return orderFeedback;
    }

    public async Task<bool> UpdateAsync(OrderFeedback orderFeedback)
    {
        var entity = await _context.OrderFeedbacks.FindAsync(orderFeedback.Id);
        if (entity == null)
        {
            return false;
        }

        entity.StudentId = orderFeedback.StudentId;
        entity.FeedbackText = orderFeedback.FeedbackText;
        entity.Stars = orderFeedback.Stars;
        entity.OrderId = orderFeedback.OrderId;

        _context.OrderFeedbacks.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.OrderFeedbacks.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.OrderFeedbacks.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}