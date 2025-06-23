using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;
    public class OrderParagraphRepository : IOrderParagraphRepository
    {
        private readonly WirinDbContext _context;

        public OrderParagraphRepository(WirinDbContext context)
        {
            _context = context;
        }

    public async Task<List<Paragraph>> GetAllParagraphsByOrderIdAsync(int orderId)
    {
        var paragraphEntities = await _context.Paragraph
            .Where(p => p.OrderId == orderId)
            .ToListAsync();

        var paragraphsDomain = paragraphEntities
            .Select(ParagraphTransformer.ToDomain)
            .ToList();

        return paragraphsDomain;
    }


    public async Task<Paragraph> GetParagraphByIdAsync(int orderId, int pageNumber)
    {
        var paragraphEntity = await _context.Paragraph
            .Where(p => p.PageNumber == pageNumber)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        if (paragraphEntity == null)
            return null;

        var paragraphDomain = ParagraphTransformer.ToDomain(paragraphEntity);
        return paragraphDomain;
    }



    public async Task SaveAsync(Paragraph paragraph)
        {
            // Transformar el párrafo a entidad
            var paragraphEntity = ParagraphTransformer.ToEntity(paragraph);

            // Agregar el párrafo al contexto
            _context.Paragraph.Add(paragraphEntity);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
            
            Paragraph p = ParagraphTransformer.ToDomain(paragraphEntity);

        }

    public async Task UpdateParagraphAsync(Paragraph paragraph)
    {
        // Buscar el párrafo original en la base de datos
        var paragraphInDB = await _context.Paragraph
            .FirstOrDefaultAsync(p => p.OrderId == paragraph.OrderId && p.PageNumber == paragraph.PageNumber);

        if (paragraphInDB == null)
            throw new Exception("Párrafo no encontrado.");

        // Actualizar el campo que cambia
        paragraphInDB.ParagraphText = paragraph.ParagraphText;
        paragraphInDB.HasError = paragraph.HasError;
        paragraphInDB.Confidence = paragraph.Confidence;

        await _context.SaveChangesAsync();
    }

}
