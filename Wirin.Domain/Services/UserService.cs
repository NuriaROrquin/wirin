using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public virtual async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public virtual async Task<User> GetUserById(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public virtual async Task<IEnumerable<User>> GetAllStudentsAsync()
        {
            return await _userRepository.GetAllStudentsAsync();
        }

        public virtual async Task<User> GetUserWithRoles(string email)
        {
            return await _userRepository.GetUserWithRoles(email);
        }

        public virtual async Task<User> GetByEmailAsync(string email)
        {
            return await _userRepository.GetUserWithRoles(email);
        }

        public virtual async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _userRepository.GetUserByUserName(userName);
        }

        public virtual async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userRepository.CheckPasswordAsync(user, password);
        }

        public virtual async Task<bool> AddUserAsync(User user, string password)
        {
            return await _userRepository.AddUserAsync(user, password);
        }

        public virtual async Task<bool> UpdateAsync(User user, string id)
        {
            return await _userRepository.UpdateAsync(user, id);
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public virtual async Task<IEnumerable<User>> getUsersByRoleAsync(string role)
        {
            return await _userRepository.GetUsersByRol(role);
        }

        public virtual string GetUserTrasabilityId(ClaimsPrincipal user)
        {
            return GetUserWithRoles(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value).Result.Id;
        }


    }
}
