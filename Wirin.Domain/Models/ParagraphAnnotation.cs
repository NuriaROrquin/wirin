namespace Wirin.Domain.Models;

public class ParagraphAnnotation
{
    public int Id { get; set; }
    public string AnnotationText { get; set; }
    public DateTime CreationDate { get; set; }
    public int ParagraphId { get; set; }
    public string? UserId { get; set; }
    public string? StudenId { get; set; }
    public int OrderId { get; set; }
}
