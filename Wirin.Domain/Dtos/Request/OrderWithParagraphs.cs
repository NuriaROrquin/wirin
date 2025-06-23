using Wirin.Domain.Models;

namespace Wirin.Domain.Dtos.Request;

public class OrderWithParagraphs
{
    public Order Order { get; set; }
    public List<Paragraph> ParagraphTexts { get; set; }
}
