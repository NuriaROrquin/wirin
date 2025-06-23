using System.ComponentModel.DataAnnotations;

namespace Wirin.Api.Dtos.Requests;

public class CreateCareerRequest
{
    public string Name { get; set; }

    public string CodDepartamento { get; set; }
}