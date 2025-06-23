using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services;

public class OrderFeedbackService
{
    private readonly IOrderFeedbackRepository _orderFeedbackRepository;

    public OrderFeedbackService(IOrderFeedbackRepository orderFeedbackRepository)
    {
        _orderFeedbackRepository = orderFeedbackRepository;
    }

    public virtual async Task<IEnumerable<OrderFeedback>> GetAllOrderFeedbacksAsync()
    {
        return await _orderFeedbackRepository.GetAllAsync();
    }

    public virtual async Task<OrderFeedback> GetOrderFeedbackByIdAsync(int id)
    {
        return await _orderFeedbackRepository.GetByIdAsync(id);
    }

    public virtual async Task<OrderFeedback> AddOrderFeedbackAsync(OrderFeedback orderFeedback)
    {
        try
        {
            ValidateOrderFeedback(orderFeedback);
            return await _orderFeedbackRepository.AddAsync(orderFeedback);
        }
        catch (ArgumentException)
        {
            return null;
        }

    }

    public virtual async Task<bool> UpdateOrderFeedbackAsync(int id, OrderFeedback orderFeedback)
    {
        try
        {
            ValidateOrderFeedback(orderFeedback);
            var existingFeedback = await _orderFeedbackRepository.GetByIdAsync(id);
            if (existingFeedback == null)
            {
                return false; // Not found
            }

            // Update properties of existingFeedback with values from orderFeedback
            existingFeedback.StudentId = orderFeedback.StudentId;
            existingFeedback.FeedbackText = orderFeedback.FeedbackText;
            existingFeedback.Stars = orderFeedback.Stars;
            existingFeedback.OrderId = orderFeedback.OrderId;

            return await _orderFeedbackRepository.UpdateAsync(existingFeedback);
        }
        catch (ArgumentException)
        {
            return false;
        }
       
    }

    public virtual async Task<bool> DeleteOrderFeedbackAndHandleNotFoundAsync(int id)
    {
        var existingFeedback = await _orderFeedbackRepository.GetByIdAsync(id);
        if (existingFeedback == null)
        {
            return false; // Not found
        }
        return await _orderFeedbackRepository.DeleteAsync(id);
    }

    protected virtual void ValidateOrderFeedback(OrderFeedback orderFeedback)
    {
        if (orderFeedback.Stars < 1 || orderFeedback.Stars > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(orderFeedback.Stars), "Las estrellas deben estar entre 1 y 5.");
        }
        if (string.IsNullOrWhiteSpace(orderFeedback.FeedbackText))
        {
            throw new ArgumentException("El texto de feedback no puede estar vacío.", nameof(orderFeedback.FeedbackText));
        }
    }
}