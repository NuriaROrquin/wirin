using Wirin.Domain.Dtos.User;
using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetUserByIdAsync(string id);
    Task<IEnumerable<User>> GetAllStudentsAsync();
    Task<User> GetUserWithRoles(string email);

    Task<User> GetUserByUserName(string userName);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> AddUserAsync(User user, string password);

    Task<bool> UpdateAsync(User user, string id);

    Task<bool> DeleteAsync(string id);

    Task<IEnumerable<User>> GetUsersByRol(string role);
}
