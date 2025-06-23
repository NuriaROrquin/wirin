namespace Wirin.Domain.Models;

public class Career
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CodDepartamento { get; set; }
    public List<Subject> Subjects { get; set; } = new List<Subject>();
}