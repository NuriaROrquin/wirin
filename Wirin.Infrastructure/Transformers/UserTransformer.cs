using Wirin.Domain.Dtos.User;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class UserTransformer
{
    public static User ToDomain(UserEntity user)
    {
        return new User
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public static User ToDomain(UserWithRolesDto user)
    {
        return new User
        {
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email,
            Roles = user.Roles.AsEnumerable().Select(r => r).ToList()
        };
    }
}
