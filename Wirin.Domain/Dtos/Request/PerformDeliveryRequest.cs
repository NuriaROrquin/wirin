using Wirin.Domain.Models;

namespace Wirin.Domain.Dtos.Requests;

public class PerformDeliveryRequest
{
    public List<OrderSequence> SelectedOrders { get; set; }
    public string StudentId { get; set; }
}
