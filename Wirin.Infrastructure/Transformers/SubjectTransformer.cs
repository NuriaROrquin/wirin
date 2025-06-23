using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public static class SubjectTransformer
{
    public static Subject ToDomain(SubjectEntity entity)
    {
        if (entity == null) return null;
        return new Subject
        {
            Id = entity.Id,
            Name = entity.Name,
            CareerId = entity.CareerId,
            // Career navigation property is not typically mapped here to avoid circular dependencies in simple transformers
            // It's usually populated by the service layer or EF Core eager/explicit loading
        };
    }

    public static SubjectEntity ToEntity(Subject model)
    {
        if (model == null) return null;
        return new SubjectEntity
        {
            Id = model.Id,
            Name = model.Name,
            CareerId = model.CareerId
        };
    }
}