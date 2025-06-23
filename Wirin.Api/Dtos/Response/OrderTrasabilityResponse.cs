namespace Wirin.Api.Dtos.Response;

    public class OrderTrasabilityResponse
    {
        public int OrderId { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public DateTime ProcessedAt { get; set; }

    public static OrderTrasabilityResponse FromDomain(OrderTrasability trasability) {

        return new OrderTrasabilityResponse
        {
            OrderId = trasability.OrderId,
            Action = trasability.Action,
            UserId = trasability.UserId,
            ProcessedAt = trasability.ProcessedAt
        };
    }
    public static List<OrderTrasabilityResponse> ListFromDomain(List<OrderTrasability> trasability)
    {
        return trasability.Select(t => FromDomain(t)).ToList();
    }

}

