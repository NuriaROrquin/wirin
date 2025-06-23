using System.Collections.Generic;

namespace Wirin.Api.Dtos.Response;

public class CareerResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CodDepartamento { get; set; }
    public List<SubjectResponse> Subjects { get; set; }

    public static CareerResponse FromDomain(Wirin.Domain.Models.Career career)
    {
        if (career == null) return null;
        return new CareerResponse
        {
            Id = career.Id,
            Name = career.Name,
            CodDepartamento = career.CodDepartamento,
            Subjects = career.Subjects?.Select(SubjectResponse.FromDomain).ToList() ?? new List<SubjectResponse>()
        };
    }

    public static List<CareerResponse> ListFromDomain(IEnumerable<Wirin.Domain.Models.Career> careers)
    {
        return careers.Select(FromDomain).ToList();
    }
}