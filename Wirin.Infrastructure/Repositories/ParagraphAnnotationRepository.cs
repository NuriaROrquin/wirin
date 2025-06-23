using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class ParagraphAnnotationRepository : IParagraphAnnotationRepository
{

    private readonly WirinDbContext _context;

    public ParagraphAnnotationRepository(WirinDbContext context)
    {
        _context = context;
    }
    public async Task<List<ParagraphAnnotation>> GetAllParagraphAnnotationsByParagraphIdAsync(int orderId, int pageNumber)
    {
        var annotationEntities = await _context.ParagraphAnnotations
            .Where(pa => pa.OrderId == orderId && pa.ParagraphId == pageNumber)
            .ToListAsync();

        var annotationsDomain = annotationEntities
            .Select(ParagraphAnnotationTransformer.ToDomain)
            .ToList();

        return annotationsDomain;
    }

    public async Task SaveParagraphAnnotationAsync(ParagraphAnnotation paragraphAnnotation)
    {
        paragraphAnnotation.CreationDate = DateTime.UtcNow;
        var entity = ParagraphAnnotationTransformer.ToEntity(paragraphAnnotation);
        await _context.ParagraphAnnotations.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}
