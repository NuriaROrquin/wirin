using System.ComponentModel.DataAnnotations;

namespace Wirin.Api.Dtos.Requests;

public class UpdateCareerRequest
{
    public string Name { get; set; }

    public string CodDepartamento { get; set; }
}