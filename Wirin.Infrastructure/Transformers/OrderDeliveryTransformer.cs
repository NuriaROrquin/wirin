using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;


namespace Wirin.Infrastructure.Transformers;

public class OrderDeliveryTransformer
{
    public static OrderDelivery ToDomain(OrderDeliveryEntity orderDelivery)
    {
        if (orderDelivery == null) return null;


        return new OrderDelivery
        {
            Id = orderDelivery.Id,
            DeliveryDate = orderDelivery?.DeliveryDate,
            Status = orderDelivery.Status,
            UserId = orderDelivery.UserId,
            StudentId = orderDelivery.StudentUserId,
            Title = orderDelivery.Title
        };
    }

    public static OrderDeliveryEntity ToEntity(OrderDelivery orderDelivery)
    {
        if (orderDelivery == null) return null;

        return new OrderDeliveryEntity
        {
            Id = orderDelivery.Id,
            DeliveryDate = orderDelivery?.DeliveryDate,
            Status = orderDelivery.Status,
            UserId = orderDelivery.UserId,
            StudentUserId = orderDelivery.StudentId,
            Title = orderDelivery.Title
        };
    }

    public static OrderDelivery ToDomain(OrderDeliveryEntity orderDelivery, List<OrderEntity>? orders = null)
    {
        if (orderDelivery == null) return null;


        return new OrderDelivery
        {
            Id = orderDelivery.Id,
            DeliveryDate = orderDelivery?.DeliveryDate,
            Status = orderDelivery.Status,
            UserId = orderDelivery.UserId,
            StudentId = orderDelivery.StudentUserId,
            Title = orderDelivery.Title,
            Orders = orders?.Select(OrderTransformer.ToDomain).ToList() ?? new List<Order>()
        };
    }
}
