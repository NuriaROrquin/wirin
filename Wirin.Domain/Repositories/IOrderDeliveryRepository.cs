using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IOrderDeliveryRepository
{
    Task<List<OrderDelivery>> GetAllByUserIdAsync(string studentUserId);
    Task<List<OrderDelivery>> GetAll();
    Task<List<OrderDelivery>> GetAllWithOrders();
    Task<List<OrderDelivery>> GetAllWithOrdersByStudentId(string studentUserId);
    Task<OrderDelivery> GetByIdAsync(int id);
    Task AddAsync(OrderDelivery orderDelivery);
    Task<OrderDelivery> AddNewDeliveryAsync(string studentId, string userId);
    Task UpdateAsync(OrderDelivery orderDelivery);
    Task DeleteAsync(int id);
    
}
