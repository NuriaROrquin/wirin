using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class ParagraphAnnotationTransformer
{
    public static ParagraphAnnotationEntity ToEntity(ParagraphAnnotation paragraphAnnotation)
    {
        return new ParagraphAnnotationEntity
        {
            Id = paragraphAnnotation.Id,
            AnnotationText = paragraphAnnotation.AnnotationText,
            CreationDate = paragraphAnnotation.CreationDate,
            ParagraphId = paragraphAnnotation.ParagraphId,
            UserId = paragraphAnnotation.UserId,
            StudenId = paragraphAnnotation.StudenId,
            OrderId = paragraphAnnotation.OrderId
        };
    }

    public static ParagraphAnnotation ToDomain(ParagraphAnnotationEntity paragraphAnnotation)
    {
        return new ParagraphAnnotation
        {
            Id = paragraphAnnotation.Id,
            AnnotationText = paragraphAnnotation.AnnotationText,
            CreationDate = paragraphAnnotation.CreationDate,
            ParagraphId = paragraphAnnotation.ParagraphId,
            UserId = paragraphAnnotation.UserId,
            StudenId = paragraphAnnotation.StudenId,
            OrderId = paragraphAnnotation.OrderId
        };
    }
}