using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Wirin.Domain.Dtos.User;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly UserManager<UserEntity> _userManager;
    private readonly WirinDbContext _context;

    public UserRepository(UserManager<UserEntity> userManager, WirinDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            var users = await _context.Users
                .Select(user => new User
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .ToList()
                })
                .ToListAsync();

            return users;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los usuarios con sus roles: " + ex.Message, ex);
        }
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return null;
        }
        return new User
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Roles = await GetRolesOf(user),
            Password = user.PasswordHash
        };
    }

    private async Task<IEnumerable<string>> GetRolesOf(UserEntity user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<User> GetUserWithRoles(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
       
        if (user == null)
        {
            return null;
        }

        var roles = await GetRolesOf(user);
        
        return new User
        {
            Id = user.Id,
            UserName = user.UserName,
            Roles = roles,
            Email = user.Email,
            FullName = user.FullName,
            Password = user.PasswordHash,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        var userEntity = await _userManager.FindByEmailAsync(user.Email);
        if (userEntity == null)
        {
            return false;
        }
        return await _userManager.CheckPasswordAsync(userEntity, password);
    }

    public async Task<bool> AddUserAsync(User user, string password)
    {
        var userEntity = new UserEntity
        {
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber
        };

        var result = await _userManager.CreateAsync(userEntity, password);
        if (!result.Succeeded) return false;

        if (user.Roles != null && user.Roles.Any())
        {
            var roleResult = await _userManager.AddToRolesAsync(userEntity, user.Roles);
            if (!roleResult.Succeeded) return false;
        }

        return true;
    }

    public async Task<IEnumerable<User>> GetAllStudentsAsync()
    {
        try
        {

            var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Alumno");

            if (studentRole == null)
                return Enumerable.Empty<User>();

            // Obtenemos todos los user-role mappings para ese RoleId
            var userRoleMappings = await _context.UserRoles
                .Where(ur => ur.RoleId == studentRole.Id)
                .ToListAsync(); // IEnumerable<IdentityUserRole<string>>

            var userIds = userRoleMappings.Select(ur => ur.UserId).Distinct().ToList();

            // Cargamos los usuarios
            var users = _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(UserTransformer.ToDomain)
                .ToList();


            return users;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los alumnos con sus roles: " + ex.Message, ex);
        }
    }

    public async Task<bool> UpdateAsync(User user, string id)
    {
        var userEntity = await _userManager.FindByIdAsync(id);
        if (userEntity == null) return false;

        userEntity.UserName = user.UserName;
        userEntity.Email = user.Email;
        userEntity.FullName = user.FullName;
        userEntity.PhoneNumber = user.PhoneNumber;

        var result = await _userManager.UpdateAsync(userEntity);
        if (!result.Succeeded) return false;

        if (user.Roles != null && user.Roles.Any())
        {
            var existingRoles = await _userManager.GetRolesAsync(userEntity);
            await _userManager.RemoveFromRolesAsync(userEntity, existingRoles);
            await _userManager.AddToRolesAsync(userEntity, user.Roles);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var userEntity = await _userManager.FindByIdAsync(id);
        if (userEntity == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(userEntity);
        return result.Succeeded;
    }

    public async Task<IEnumerable<User>> GetUsersByRol(string role)
    {
        var userIds = await _context.UserRoles
            .Where(ur => _context.Roles.Any(r => r.Name == role && r.Id == ur.RoleId))
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync();

        return await _context.Users
            .Where(user => userIds.Contains(user.Id))
            .Select(user => new User
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = _context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name)
                        .ToList()
            })
            .ToListAsync();
    }

    public async Task<User> GetUserByUserName(string userName)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            return null;
        }

        return new User
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Roles = await GetRolesOf(user),
            Password = user.PasswordHash
        };
    }
}
