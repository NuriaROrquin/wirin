using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wirin.Infrastructure.Entities;

public class StudentDeliveryEntity
{
    [Key]
    public int Id { get; set; }
    public string StudentId { get; set; }
    public int OrderDeliveryId { get; set; }

    public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
}
