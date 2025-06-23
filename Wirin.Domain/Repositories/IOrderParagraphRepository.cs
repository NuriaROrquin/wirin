
using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;
public interface IOrderParagraphRepository
{
    Task<List<Paragraph>> GetAllParagraphsByOrderIdAsync(int orderId);
    Task SaveAsync(Paragraph paragraph);
    Task UpdateParagraphAsync(Paragraph paragraph);
    Task<Paragraph> GetParagraphByIdAsync(int orderId, int page);

}


