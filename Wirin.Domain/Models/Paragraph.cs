namespace Wirin.Domain.Models;

public class Paragraph
{
    public int OrderId { get; set; }
    public string ParagraphText { get; set; }
    public int PageNumber { get; set; }
    public bool? HasError { get; set; }
    public double Confidence { get; set; }
    public List<ParagraphAnnotation> Annotations { get; set; } = new List<ParagraphAnnotation>();
}