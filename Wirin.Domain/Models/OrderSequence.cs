namespace Wirin.Domain.Models;

public class OrderSequence
{
    public int Id { get; set; }
    public int OrderId { get; set; } 
    public int Order { get; set; }
    public int OrderDeliveryId { get; set; }
}
