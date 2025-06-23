using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class OrderTransformer
{
    public static Order ToDomain(OrderEntity order)
    {
        if (order == null)
            return null;
            
        return new Order
        {
            Id = order.Id,
            Name = order.Name,
            Subject = order.Subject,
            Description = order.Description,
            AuthorName = order.AuthorName,
            rangePage = order.rangePage,
            IsPriority = order.IsPriority,
            Status = order.Status,
            CreationDate = order.CreationDate,
            LimitDate = order.LimitDate,
            CreatedByUserId = order.CreatedByUserId,
            FilePath = order.FilePath,
            VoluntarioId = order.VoluntarioId,
            AlumnoId = order.AlumnoId,
            RevisorId = order.RevisorId,
            DelivererId = order?.DelivererId
        };
    }

    public static OrderEntity ToEntity(Order order)
    {
        return new OrderEntity
        {
            Id = order.Id,
            Name = order.Name,
            Subject = order.Subject,
            Description = order.Description,
            AuthorName = order.AuthorName,
            rangePage = order.rangePage,
            IsPriority = order.IsPriority,
            Status = order.Status,
            CreationDate = order.CreationDate,
            LimitDate = order.LimitDate,
            CreatedByUserId = order.CreatedByUserId,
            FilePath = order.FilePath,
            VoluntarioId = order.VoluntarioId,
            AlumnoId = order.AlumnoId,
            RevisorId = order.RevisorId,
            DelivererId = order?.DelivererId
        };
    }
}