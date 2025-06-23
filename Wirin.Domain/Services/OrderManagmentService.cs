using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Wirin.Domain.Dtos.Request;

namespace Wirin.Infrastructure.Services;
public class OrderManagmentService
{
    private readonly IOrderManagmentRepository _OrderManagmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDeliveryRepository _orderDeliveryRepository;
    private readonly IOrderSequenceRepository _orderSequenceRepository;
    private readonly IOrderTrasabilityRepository _orderTrasabilityRepository;
    private readonly IOrderParagraphRepository _orderParagraphRepository;


    public OrderManagmentService( IOrderParagraphRepository orderParagraphRepository ,IOrderManagmentRepository OrderManagmentRepository, IOrderRepository orderRepository, IOrderDeliveryRepository orderDeliveryRepository, IOrderSequenceRepository orderSequenceRepository, IOrderTrasabilityRepository orderTrasabilityRepository)
    {
        _OrderManagmentRepository = OrderManagmentRepository;
        _orderRepository = orderRepository;
        _orderDeliveryRepository = orderDeliveryRepository;
        _orderSequenceRepository = orderSequenceRepository;
        _orderTrasabilityRepository = orderTrasabilityRepository;
        _orderParagraphRepository = orderParagraphRepository;

    }

    public virtual async Task<List<Order>> GetAllOrderByStatus(string status)
    {

        return await _OrderManagmentRepository.GetAllOrderByStatus(status);

    }

    public virtual Task<List<Order>> GetAllByUserAssigned(string voluntarioId)
    {
        return _OrderManagmentRepository.GetAllByUserAssigned(voluntarioId);

    }

    public virtual async Task SaveRevisorId(int OrderId, string revisorId, string trasabilityUserId)
    {
        Order order = await _orderRepository.GetByIdAsync(OrderId);
        order.RevisorId = revisorId;

        await _OrderManagmentRepository.UpdateAsync(order);

        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = order.Id,
            UserId = trasabilityUserId,
            Action = $"Asignado a voluntario",
            ProcessedAt = DateTime.UtcNow
        });

        return;
    }

    public virtual async Task SaveVoluntarioId(int OrderId, string voluntarioId, string trasabilityUserId)
    {
        Order order = await _orderRepository.GetByIdAsync(OrderId);
        order.VoluntarioId = voluntarioId;

        await _OrderManagmentRepository.UpdateAsync(order);

        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = order.Id,
            UserId = trasabilityUserId,
            Action = $"Asignado a voluntario",
            ProcessedAt = DateTime.UtcNow
        });

        return;
    }

    public virtual async Task ChangeState(int OrderId, string newState, string trasabilityUserId)
    {
        Order order = await _orderRepository.GetByIdAsync(OrderId);

        // Diccionario que define las transiciones válidas
        var validTransitions = new Dictionary<string, string[]>
        {
            { "En Proceso", new[] { "Pendiente", "Denegada" } },
            { "En Revisión", new[] { "En Proceso" } },
            { "Aprobada", new[] { "En Revisión"} },
            { "Completada", new[] { "Aprobada" } },
            { "Denegada", new[] { "En Revisión", "Aprobada" } },
            { "Entregada", new[] { "Completada" } }
        };

        // Validar si la transición es permitida
       /* if (!validTransitions.ContainsKey(newState) || !validTransitions[newState].Contains(order.Status))
        {
            throw new Exception($"La orden no está en un estado válido para ser marcada como {newState}.");
        }*/

        // Actualizar estado y guardar en la base de datos
        order.Status = newState;
        
        // Si el estado es "Completada", cambiar la prioridad a false
        if (newState == "Completada" || newState == "Entregada")
        {
            order.IsPriority = false;
        }
        
        await _OrderManagmentRepository.UpdateAsync(order);

        // Guardar trazabilidad
        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = order.Id,
            UserId = trasabilityUserId,
            Action = $"Estado cambiado a {newState}",
            ProcessedAt = DateTime.UtcNow
        });
    }


    public virtual async Task<List<OrderWithParagraphs>> GetAllByStudent(string studentId)
    {
        var orders = await _OrderManagmentRepository.GetAllByStudentAsync(studentId);
        var ordersWithParagraphs = new List<OrderWithParagraphs>();

        foreach (var order in orders)
        {
            var paragraphs = await _orderParagraphRepository.GetAllParagraphsByOrderIdAsync(order.Id); // 🔥 Usa `await`

            ordersWithParagraphs.Add(new OrderWithParagraphs
            {
                Order = order,
                ParagraphTexts = paragraphs 
            });
        }

        return ordersWithParagraphs;
    }
}
