using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public static class OrderFeedbackTransformer
{
    public static OrderFeedback ToDomain(OrderFeedbackEntity entity)
    {
        if (entity == null) return null;

        return new OrderFeedback
        {
            Id = entity.Id,
            StudentId = entity.StudentId,
            FeedbackText = entity.FeedbackText,
            Stars = entity.Stars,
            OrderId = entity.OrderId
        };
    }

    public static OrderFeedbackEntity ToEntity(OrderFeedback model)
    {
        if (model == null) return null;

        return new OrderFeedbackEntity
        {
            Id = model.Id,
            StudentId = model.StudentId,
            FeedbackText = model.FeedbackText,
            Stars = model.Stars,
            OrderId = model.OrderId
        };
    }

    public static IEnumerable<OrderFeedback> ToDomainList(IEnumerable<OrderFeedbackEntity> entities)
    {
        return entities?.Select(ToDomain) ?? Enumerable.Empty<OrderFeedback>();
    }
}