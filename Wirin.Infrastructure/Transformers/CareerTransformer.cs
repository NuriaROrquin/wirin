using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;
using System.Linq;

namespace Wirin.Infrastructure.Transformers;

public static class CareerTransformer
{
    public static Career ToDomain(CareerEntity entity)
    {
        if (entity == null) return null;
        return new Career
        {
            Id = entity.Id,
            Name = entity.Name,
            CodDepartamento = entity.CodDepartamento,
            Subjects = entity.Subjects?.Select(SubjectTransformer.ToDomain).ToList() ?? new List<Subject>()
        };
    }

    public static CareerEntity ToEntity(Career model)
    {
        if (model == null) return null;
        return new CareerEntity
        {
            Id = model.Id,
            Name = model.Name,
            CodDepartamento = model.CodDepartamento,
            // Subjects are typically managed separately or handled by EF Core relationships
        };
    }
}