using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class OrderTrasabilityTransformer
{  
    public static OrderTrasability ToDomain(OrderTrasabilityEntity message)
    {
          return new OrderTrasability
          {
              OrderId = message.OrderId,
              Action = message.Action,
              UserId = message.UserId,
              ProcessedAt = message.ProcessedAt
          };
    }

    public static OrderTrasabilityEntity ToEntity(OrderTrasability message)
    {
        return new OrderTrasabilityEntity
        {
            OrderId = message.OrderId,
            Action = message.Action,
            UserId = message.UserId,
            ProcessedAt = message.ProcessedAt

        };
    }
}
