using Wirin.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wirin.Domain.Repositories;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllAsync();
    Task<Subject> GetByIdAsync(int id);
    Task<IEnumerable<Subject>> GetByCareerIdAsync(int careerId);
    Task AddAsync(Subject subject);
    Task UpdateAsync(Subject subject);
    Task DeleteAsync(int id);
}