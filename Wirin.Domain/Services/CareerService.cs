using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wirin.Domain.Services;

public class CareerService
{
    private readonly ICareerRepository _careerRepository;

    public CareerService(ICareerRepository careerRepository)
    {
        _careerRepository = careerRepository;
    }

    public virtual async Task<IEnumerable<Career>> GetAllCareersAsync()
    {
        return await _careerRepository.GetAllAsync();
    }

    public virtual async Task<Career> GetCareerByIdAsync(int id)
    {
        return await _careerRepository.GetByIdAsync(id);
    }

    public virtual async Task AddCareerAsync(Career career)
    {
        // Add validation or business logic if needed
        await _careerRepository.AddAsync(career);
    }

    public virtual async Task UpdateCareerAsync(Career career)
    {
        // Add validation or business logic if needed
        await _careerRepository.UpdateAsync(career);
    }

    public virtual async Task DeleteCareerAsync(int id)
    {
        await _careerRepository.DeleteAsync(id);
    }
}