using System.ComponentModel.DataAnnotations;

namespace Wirin.Infrastructure.Entities;
public class ParagraphEntity
{
    [Key]
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ParagraphText { get; set; }
    public int PageNumber { get; set; }
    public bool? HasError { get; set; }
    public double? Confidence { get; set; }
}