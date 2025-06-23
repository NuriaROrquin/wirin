using System.Collections.Generic;
using System.Linq;
using Wirin.Domain.Dtos.User;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;
using Xunit;

public class UserTransformerTests
{
    [Fact]
    public void ToDomain_FromUserEntity_ShouldMapCorrectly()
    {
        var userEntity = new UserEntity
        {
            Id = "abc123",
            UserName = "usuario1",
            FullName = "Usuario Uno",
            Email = "user1@mail.com",
            Roles = new List<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>
            {
                new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { RoleId = "Admin", UserId = "abc123" },
                new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { RoleId = "User", UserId = "abc123" }
            }
        };

        var result = UserTransformer.ToDomain(userEntity);

        Assert.NotNull(result);
        Assert.Equal("abc123", result.Id);
        Assert.Equal("usuario1", result.UserName);
        Assert.Equal("Usuario Uno", result.FullName);
        Assert.Equal("user1@mail.com", result.Email);

        // El transformer actual NO mapea roles en UserEntity a domain (según el código que diste),
        // por lo que result.Roles será null o vacío, entonces no verificamos roles aquí.
    }



    [Fact]
    public void ToDomain_FromUserWithRolesDto_ShouldMapPropertiesExceptId()
    {
        var dto = new UserWithRolesDto
        {
            UserName = "usuario2",
            FullName = "Usuario Dos",
            Email = "user2@mail.com",
            Roles = new List<string> { "RoleA", "RoleB" }
        };

        var result = UserTransformer.ToDomain(dto);

        Assert.NotNull(result);
        Assert.Null(result.Id); // Porque transformer no asigna Id desde DTO
        Assert.Equal("usuario2", result.UserName);
        Assert.Equal("Usuario Dos", result.FullName);
        Assert.Equal("user2@mail.com", result.Email);
        Assert.NotNull(result.Roles);
        Assert.Contains("RoleA", result.Roles);
        Assert.Contains("RoleB", result.Roles);
    }

    [Fact]
    public void ToDomain_FromUserEntity_ValidInput_ReturnsUser()
    {
        var userEntity = new UserEntity
        {
            Id = "123",
            FullName = "John Doe",
            UserName = "johndoe",
            Email = "john@example.com"
        };

        var result = UserTransformer.ToDomain(userEntity);

        Assert.NotNull(result);
        Assert.Equal("123", result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("johndoe", result.UserName);
        Assert.Equal("john@example.com", result.Email);
    }

}
