using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IParagraphAnnotationRepository
{
    Task SaveParagraphAnnotationAsync(ParagraphAnnotation paragraphAnnotation);
    Task<List<ParagraphAnnotation>> GetAllParagraphAnnotationsByParagraphIdAsync(int orderId, int pageNumber);
}
