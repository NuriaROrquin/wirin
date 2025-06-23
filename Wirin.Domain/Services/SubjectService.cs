using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wirin.Domain.Services;

public class SubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public virtual async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
    {
        return await _subjectRepository.GetAllAsync();
    }

    public virtual async Task<Subject> GetSubjectByIdAsync(int id)
    {
        return await _subjectRepository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<Subject>> GetSubjectsByCareerIdAsync(int careerId)
    {
        return await _subjectRepository.GetByCareerIdAsync(careerId);
    }

    public virtual async Task AddSubjectAsync(Subject subject)
    {
        // Add validation or business logic if needed
        await _subjectRepository.AddAsync(subject);
    }

    public virtual async Task UpdateSubjectAsync(Subject subject)
    {
        // Add validation or business logic if needed
        await _subjectRepository.UpdateAsync(subject);
    }

    public virtual async Task DeleteSubjectAsync(int id)
    {
        await _subjectRepository.DeleteAsync(id);
    }
}