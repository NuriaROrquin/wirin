using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

    public class ParagraphTransformer
    {
    public static Paragraph ToDomain(ParagraphEntity paragraph)
    {
        return new Paragraph
        {
            OrderId = paragraph.OrderId,
            ParagraphText = paragraph.ParagraphText,
            PageNumber = paragraph.PageNumber,
            HasError = paragraph.HasError,
            Confidence = paragraph.Confidence ?? 0.0,
        };
    }

    public static ParagraphEntity ToEntity(Paragraph paragraph)
    {
        return new ParagraphEntity
        {
            OrderId = paragraph.OrderId,
            ParagraphText = paragraph.ParagraphText,
            PageNumber = paragraph.PageNumber,
            HasError = paragraph.HasError,
            Confidence = paragraph.Confidence,
        };
    }
}
