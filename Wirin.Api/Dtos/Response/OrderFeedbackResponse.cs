using Wirin.Domain.Models;

namespace Wirin.Api.Dtos.Response;

public class OrderFeedbackResponse
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public string FeedbackText { get; set; }
    public int Stars { get; set; }
    public int OrderId { get; set; }

    public static OrderFeedbackResponse FromDomain(OrderFeedback model)
    {
        if (model == null) return null;

        return new OrderFeedbackResponse
        {
            Id = model.Id,
            StudentId = model.StudentId,
            FeedbackText = model.FeedbackText,
            Stars = model.Stars,
            OrderId = model.OrderId
        };
    }

    public static IEnumerable<OrderFeedbackResponse> ListFromDomain(IEnumerable<OrderFeedback> models)
    {
        return models?.Select(FromDomain) ?? Enumerable.Empty<OrderFeedbackResponse>();
    }
}