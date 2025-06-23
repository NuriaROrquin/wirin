using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wirin.Infrastructure.Entities;

[Table("OrderFeedbacks")]
public class OrderFeedbackEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; }

    [Required]
    public string FeedbackText { get; set; }

    [Required]
    [Range(1, 5)]
    public int Stars { get; set; }

    [Required]
    public int OrderId { get; set; }
}