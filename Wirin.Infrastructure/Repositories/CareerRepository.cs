using Microsoft.EntityFrameworkCore;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wirin.Infrastructure.Repositories;

public class CareerRepository : ICareerRepository
{
    private readonly WirinDbContext _context;

    public CareerRepository(WirinDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Career>> GetAllAsync()
    {
        var careerEntities = await _context.Careers
                                         .Include(c => c.Subjects)
                                         .ToListAsync();
        return careerEntities.Select(CareerTransformer.ToDomain);
    }

    public async Task<Career> GetByIdAsync(int id)
    {
        var careerEntity = await _context.Careers
                                       .Include(c => c.Subjects)
                                       .FirstOrDefaultAsync(c => c.Id == id);
        return CareerTransformer.ToDomain(careerEntity);
    }

    public async Task AddAsync(Career career)
    {
        var careerEntity = CareerTransformer.ToEntity(career);
        await _context.Careers.AddAsync(careerEntity);
        await _context.SaveChangesAsync();
        career.Id = careerEntity.Id; // Update the model with the generated ID
    }

    public async Task UpdateAsync(Career career)
    {
        var careerEntity = await _context.Careers.FindAsync(career.Id);
        if (careerEntity != null)
        {
            careerEntity.Name = career.Name;
            careerEntity.CodDepartamento = career.CodDepartamento;
            // EF Core tracks changes, so SaveChangesAsync will update it.
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var careerEntity = await _context.Careers.FindAsync(id);
        if (careerEntity != null)
        {
            _context.Careers.Remove(careerEntity);
            await _context.SaveChangesAsync();
        }
    }
}