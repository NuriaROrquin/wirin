namespace Wirin.Domain.Models;

public class OrderFeedback
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public string FeedbackText { get; set; }
    public int Stars { get; set; }
    public int OrderId { get; set; }
}