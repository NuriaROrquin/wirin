using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wirin.Infrastructure.Entities
{
    public class OrderTrasabilityEntity
    {

        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
