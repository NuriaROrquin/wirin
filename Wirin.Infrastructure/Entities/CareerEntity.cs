using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wirin.Infrastructure.Entities;

[Table("Careers")]
public class CareerEntity
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string CodDepartamento { get; set; }

    public virtual ICollection<SubjectEntity> Subjects { get; set; } = new List<SubjectEntity>();
}