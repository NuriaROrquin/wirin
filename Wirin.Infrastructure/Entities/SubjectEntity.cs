using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wirin.Infrastructure.Entities;

[Table("Subjects")]
public class SubjectEntity
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public int CareerId { get; set; }

    [ForeignKey("CareerId")]
    public virtual CareerEntity Career { get; set; }
}