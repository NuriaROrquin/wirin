
using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IStudentDeliveryRepository
{
    Task AddStudentDeliveryAsync(StudentDelivery studentDelivery);
    Task<IEnumerable<StudentDelivery>> GetAllStudentDeliveryAsync();
    Task<IEnumerable<StudentDelivery>> GetStudentDeliveriesAsync();
}
