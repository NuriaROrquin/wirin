using Wirin.Domain.Models;

namespace Wirin.Domain.Dtos.Request;

public class OrderParagraphResponse
{
    public int OrderId { get; set; }
    public string ParagraphText { get; set; }
    public int PageNumber { get; set; }

}