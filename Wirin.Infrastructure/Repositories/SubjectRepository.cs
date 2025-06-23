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

public class SubjectRepository : ISubjectRepository
{
    private readonly WirinDbContext _context;

    public SubjectRepository(WirinDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subject>> GetAllAsync()
    {
        var subjectEntities = await _context.Subjects
                                          .Include(s => s.Career)
                                          .ToListAsync();
        return subjectEntities.Select(SubjectTransformer.ToDomain);
    }

    public async Task<Subject> GetByIdAsync(int id)
    {
        var subjectEntity = await _context.Subjects
                                        .Include(s => s.Career)
                                        .FirstOrDefaultAsync(s => s.Id == id);
        return SubjectTransformer.ToDomain(subjectEntity);
    }

    public async Task<IEnumerable<Subject>> GetByCareerIdAsync(int careerId)
    {
        var subjectEntities = await _context.Subjects
                                          .Where(s => s.CareerId == careerId)
                                          .Include(s => s.Career)
                                          .ToListAsync();
        return subjectEntities.Select(SubjectTransformer.ToDomain);
    }

    public async Task AddAsync(Subject subject)
    {
        var subjectEntity = SubjectTransformer.ToEntity(subject);
        await _context.Subjects.AddAsync(subjectEntity);
        await _context.SaveChangesAsync();
        subject.Id = subjectEntity.Id; // Update the model with the generated ID
    }

    public async Task UpdateAsync(Subject subject)
    {
        var subjectEntity = await _context.Subjects.FindAsync(subject.Id);
        if (subjectEntity != null)
        {
            subjectEntity.Name = subject.Name;
            subjectEntity.CareerId = subject.CareerId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var subjectEntity = await _context.Subjects.FindAsync(id);
        if (subjectEntity != null)
        {
            _context.Subjects.Remove(subjectEntity);
            await _context.SaveChangesAsync();
        }
    }
}