using System.ComponentModel.DataAnnotations;

namespace Wirin.Api.Dtos.Requests;

public class CreateSubjectRequest
{
    public string Name { get; set; }

    public int CareerId { get; set; }
}