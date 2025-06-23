
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services;

public class ParagraphAnnotationService
{
    private readonly IParagraphAnnotationRepository _paragraphAnnotationRepository;

    public ParagraphAnnotationService(IParagraphAnnotationRepository paragraphAnnotationRepository)
    {
        _paragraphAnnotationRepository = paragraphAnnotationRepository;
    }

    public virtual async Task<List<ParagraphAnnotation>> GetAllAnnotationsByParagraphIdAsync(OrderParagrapAnnotationsRequest request)
    {
        return await _paragraphAnnotationRepository.GetAllParagraphAnnotationsByParagraphIdAsync(request.ParagraphAnnotation.ParagraphId, request.ParagraphAnnotation.ParagraphId);
    }

    public virtual async Task SaveParagraphAnnotationAsync(OrderParagrapAnnotationsRequest request)
    {
        await _paragraphAnnotationRepository.SaveParagraphAnnotationAsync(request.ParagraphAnnotation);
    }
}
