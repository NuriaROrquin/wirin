namespace Wirin.Domain.Models;

public class OrderDelivery
{
    public string StudentId { get; set; }
    public int Id { get; set; }
    public string Status { get; set; }
    public string UserId { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string Title { get; set; }
    public List<Order>? Orders { get; set; }
    public List<Paragraph>? OrderParagraphs { get; set; }

    public string? UserName { get; set; }

    public string? StudentUserName { get; set; }
}