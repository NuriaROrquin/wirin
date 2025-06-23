namespace Wirin.Domain.Models;

public class StudentDelivery
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public int OrderDeliveryId { get; set; }
    public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
}
