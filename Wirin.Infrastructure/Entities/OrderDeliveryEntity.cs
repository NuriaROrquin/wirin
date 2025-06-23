using System.ComponentModel.DataAnnotations;

namespace Wirin.Infrastructure.Entities;

public class OrderDeliveryEntity
{
    [Key]
    public int Id { get; set; }
    public string StudentUserId { get; set; }
    public DateTime? DeliveryDate { get; set; } = DateTime.UtcNow;
    public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; }
    public string Status { get; set; }
    public string Title { get; set; }
}

