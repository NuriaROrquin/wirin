using System.ComponentModel.DataAnnotations;

namespace Wirin.Domain.Models;

public class Order
{
    [Key]
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string AuthorName { get; set; }
    public string rangePage { get; set; }
    public bool IsPriority { get; set; }
    public string Status { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? LimitDate { get; set; }
    public string? CreatedByUserId { get; set; }
    public string? FilePath { get; set; }
    public string? VoluntarioId { get; set; }
    public string? AlumnoId { get; set; }
    public string? RevisorId { get; set; }
    public int? DelivererId { get; set; }

}
