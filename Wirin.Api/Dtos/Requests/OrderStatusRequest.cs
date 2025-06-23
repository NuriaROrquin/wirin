namespace Wirin.Api.Dtos.Requests;

public class OrderChangeStatusRequest
{
    public int Id { get; set; }
    public string Status { get; set; }
}
