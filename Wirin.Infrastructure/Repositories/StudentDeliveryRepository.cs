using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class StudentDeliveryRepository : IStudentDeliveryRepository
{
    private readonly WirinDbContext _context;

    public StudentDeliveryRepository(WirinDbContext context)
    {
        _context = context;
    }
    public async Task AddStudentDeliveryAsync(StudentDelivery studentDelivery)
    {
        var studentDeliveryEntity = StudentDeliveryTransformer.ToEntity(studentDelivery);
        await _context.StudentDeliveries.AddAsync(studentDeliveryEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StudentDelivery>> GetAllStudentDeliveryAsync()
    {
        var studentDeliveryEntities = await _context.StudentDeliveries.ToListAsync();
        return studentDeliveryEntities.Select(StudentDeliveryTransformer.ToDomain).ToList();
    }

    public async Task<IEnumerable<StudentDelivery>> GetStudentDeliveriesAsync()
    {
        var studentDeliveryEntities = await _context.StudentDeliveries.ToListAsync();
        return studentDeliveryEntities.Select(StudentDeliveryTransformer.ToDomain).ToList();
    }
}
