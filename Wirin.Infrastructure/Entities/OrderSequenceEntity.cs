using System.ComponentModel.DataAnnotations;

namespace Wirin.Infrastructure.Entities;

public class OrderSequenceEntity
{
    [Key]
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int Order { get; set; }
    public int OrderDeliveryId { get; set; }
}
