using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IOrderFeedbackRepository
{
    Task<IEnumerable<OrderFeedback>> GetAllAsync();
    Task<OrderFeedback> GetByIdAsync(int id);
    Task<OrderFeedback> AddAsync(OrderFeedback orderFeedback);
    Task<bool> UpdateAsync(OrderFeedback orderFeedback);
    Task<bool> DeleteAsync(int id);
}