namespace Wirin.Domain.Models;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CareerId { get; set; }
    public Career Career { get; set; }
}