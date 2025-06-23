using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services;

public class StudentDeliveryService
{
    private readonly IStudentDeliveryRepository _studentDeliveryRepository;
    private readonly IOrderDeliveryRepository _orderDeliveryRepository;
    public StudentDeliveryService(IStudentDeliveryRepository studentDeliveryRepository, IOrderDeliveryRepository orderDeliveryRepository)
    {
        _studentDeliveryRepository = studentDeliveryRepository;
        _orderDeliveryRepository = orderDeliveryRepository;
    }
    public virtual async Task CreateStudentDelivery(StudentDelivery request)
    {
        var orderDelivery = await _orderDeliveryRepository.GetByIdAsync(request.OrderDeliveryId);
        if(orderDelivery.Status == "Completada")
        {
            orderDelivery.Status = "Entregada";
            await _orderDeliveryRepository.UpdateAsync(orderDelivery);
        }

        await _studentDeliveryRepository.AddStudentDeliveryAsync(request);
    }

    public virtual async Task<IEnumerable<StudentDelivery>> GetStudentDeliveriesAsync()
    {
        return await _studentDeliveryRepository.GetAllStudentDeliveryAsync();
    }

    public virtual async Task<IEnumerable<User>> GetUsersWithoutOrderDelivery(IEnumerable<User> students, int orderDeliveryId)
    {
        var studentDeliveries = await _studentDeliveryRepository.GetAllStudentDeliveryAsync();

        var filteredStudents = students.Where(student => !studentDeliveries
            .Any(sd => sd.StudentId == student.Id && sd.OrderDeliveryId == orderDeliveryId)
        );


        return filteredStudents;

    }

}
