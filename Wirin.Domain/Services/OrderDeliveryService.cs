using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.Requests;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services;

public class OrderDeliveryService
{
    public readonly IOrderRepository _orderRepository;
    public readonly IOrderSequenceRepository _orderSequenceRepository;
    public readonly IOrderTrasabilityRepository _orderTrasabilityRepository;
    public readonly IOrderDeliveryRepository _orderDeliveryRepository;
    public readonly IUserRepository _userRepository;

    public OrderDeliveryService(IOrderDeliveryRepository orderDeliveryRepository, IOrderRepository orderRepository, IOrderSequenceRepository orderSequenceRepository, 
        IOrderTrasabilityRepository orderTrasabilityRepository, IUserRepository userRepository)
    {
        _orderDeliveryRepository = orderDeliveryRepository;
        _orderSequenceRepository = orderSequenceRepository;
        _orderRepository = orderRepository;
        _orderTrasabilityRepository = orderTrasabilityRepository;
        _userRepository = userRepository;
    }
    public virtual async Task<List<OrderDelivery>> GetAll()
    {
        return await _orderDeliveryRepository.GetAll();
    }

    public virtual async Task<List<OrderDelivery>> GetAllWithOrders()
    {
        try
        {
            var deliveries = await _orderDeliveryRepository.GetAllWithOrders();
            foreach(var delivery in deliveries)
            {
                var user = await _userRepository.GetUserByIdAsync(delivery.UserId);
                delivery.UserName = user.FullName;
                var stundent = await _userRepository.GetUserByIdAsync(delivery.StudentId);
                delivery.StudentUserName = stundent.FullName;
            }
            
            return deliveries;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al procesar la solicitud de proyectos con órdenes.");
        }
    }

    public virtual async Task<List<OrderDelivery>> GetAllWithOrdersByStudentId(string studentId)
    {
        try
        {
            var deliveries = await _orderDeliveryRepository.GetAllWithOrdersByStudentId(studentId);
            foreach (var delivery in deliveries)
            {
                var user = await _userRepository.GetUserByIdAsync(delivery.UserId);
                delivery.UserName = user.FullName;
                var student = await _userRepository.GetUserByIdAsync(delivery.StudentId);
                delivery.StudentUserName = student.FullName;
            }

            return deliveries;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al procesar la solicitud de proyectos con órdenes por estudiante.", ex);
        }
    }



    public virtual async Task<OrderDelivery> GetById(int id)
    {
        return await _orderDeliveryRepository.GetByIdAsync(id);
    }

    public virtual async Task UpdateAsync(int id, OrderDelivery od)
    {
        await _orderDeliveryRepository.UpdateAsync(od);
    }

    public virtual async Task Create(OrderDelivery od)
    {
        await _orderDeliveryRepository.AddAsync(od);
    }

    public virtual async Task PerformDeliveryAsync(PerformDeliveryRequest req, string trasabilityUserId)
    {
        foreach (var item in req.SelectedOrders)
        {
            var order = await _orderRepository.GetByIdAsync(item.OrderId);

            if (order == null)
            {
                throw new Exception("Orden no encontrada.");
            }

        }

        var od = await _orderDeliveryRepository.AddNewDeliveryAsync(req.StudentId, trasabilityUserId);


        await _orderSequenceRepository.PerformDelivery(req.SelectedOrders, od.Id);

        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = od.Id,
            UserId = trasabilityUserId,
            Action = $"Entrega realizada para el estudiante {req.StudentId}",
            ProcessedAt = DateTime.UtcNow
        });

    }
}
