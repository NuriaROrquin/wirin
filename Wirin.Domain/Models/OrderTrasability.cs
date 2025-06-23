namespace Wirin.Api.Dtos.Response;

public class OrderTrasability
{
    public int OrderId { get; set; }
    public string Action { get; set; }
    public string UserId { get; set; }
    public DateTime ProcessedAt { get; set; }
}

