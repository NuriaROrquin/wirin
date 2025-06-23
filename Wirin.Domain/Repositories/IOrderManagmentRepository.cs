using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IOrderManagmentRepository
{
    Task<List<Order>> GetAllOrderByStatus(string status);
    Task<List<Order>> GetAllByUserAssigned(string AssignedUserId);
    Task UpdateAsync(Order order);
    Task<List<Order>> GetAllByStudentAsync(string studentId);

}
