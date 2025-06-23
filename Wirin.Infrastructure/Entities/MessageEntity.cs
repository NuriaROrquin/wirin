using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wirin.Domain.Models;
public class MessageEntity
{
    [Key]
    public int id { get; set; }
    public bool isDraft { get; set; }
    public string userFromId { get; set; }
    public string userToId { get; set; }
    public string sender { get; set; }
    public string subject { get; set; }
    public string date { get; set; }
    public string content { get; set; }
    public string? filePath { get; set; }
    public bool? responded { get; set; }
    public string? responseText { get; set; }


}