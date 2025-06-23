using Wirin.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wirin.Domain.Repositories;

public interface ICareerRepository
{
    Task<IEnumerable<Career>> GetAllAsync();
    Task<Career> GetByIdAsync(int id);
    Task AddAsync(Career career);
    Task UpdateAsync(Career career);
    Task DeleteAsync(int id);
}