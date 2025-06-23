using System.Collections.Generic;
using System.Linq;

namespace Wirin.Api.Dtos.Response;

public class SubjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CareerId { get; set; }
    // public string CareerName { get; set; } // Optional: if you want to include career name directly

    public static SubjectResponse FromDomain(Wirin.Domain.Models.Subject subject)
    {
        if (subject == null) return null;
        return new SubjectResponse
        {
            Id = subject.Id,
            Name = subject.Name,
            CareerId = subject.CareerId,
            // CareerName = subject.Career?.Name // Populate if Career is loaded
        };
    }

    public static List<SubjectResponse> ListFromDomain(IEnumerable<Wirin.Domain.Models.Subject> subjects)
    {
        return subjects.Select(FromDomain).ToList();
    }
}