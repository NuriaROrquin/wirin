using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class OrderSequenceTransformer
{
    public static OrderSequence ToDomain(OrderSequenceEntity orderSequenceEntity)
    {
        if (orderSequenceEntity == null) return null;

        return new OrderSequence
        {
            Id = orderSequenceEntity.Id,
            OrderId = orderSequenceEntity.OrderId,
           Order = orderSequenceEntity.Order,
           OrderDeliveryId = orderSequenceEntity.OrderDeliveryId,
        };
    }

    public static OrderSequenceEntity ToEntity(OrderSequence orderSequence)
    {
        if (orderSequence == null) return null;

        return new OrderSequenceEntity
        {
            Id = orderSequence.Id,
            OrderId = orderSequence.OrderId,
            Order = orderSequence.Order,
            OrderDeliveryId = orderSequence.OrderDeliveryId,
        };
    }
}
