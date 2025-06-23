using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Repositories;
using Wirin.Infrastructure.Seeders;

namespace Wirin.Tests;

public class IdentitySeederShould
{
    private IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddDbContext<WirinDbContext>(static options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddIdentity<UserEntity, Microsoft.AspNetCore.Identity.IdentityRole<string>>(options =>
        {
            // Configuración mínima para testing
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<WirinDbContext>()
        .AddDefaultTokenProviders();

        services.AddLogging(); // Necesario para Identity
        services.AddScoped<IUserRepository, UserRepository>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task CreateDefaultRolesAndSuperAdmin()
    {
        // Arrange
        var serviceProvider = BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var context = scopedServices.GetRequiredService<WirinDbContext>();
        context.Database.EnsureCreated();

        // Act
        await IdentitySeeder.SeedRolesAsync(scopedServices);
        await IdentitySeeder.SeedUsersAsync(scopedServices);
      
        var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole<string>>>();
        var userManager = scopedServices.GetRequiredService<UserManager<UserEntity>>();

        // Assert
        var roles = roleManager.Roles.Select(r => r.Name).ToList();
        Assert.Contains("Bibliotecario", roles);
        Assert.Contains("Voluntario", roles);
        Assert.Contains("Voluntario Administrativo", roles);
        Assert.Contains("Alumno", roles);

        var superAdmin = await userManager.FindByEmailAsync("mariagonzalez@biblioteca.com");
        Assert.NotNull(superAdmin);
        var superAdminRoles = await userManager.GetRolesAsync(superAdmin);
        Assert.Contains("Admin", superAdminRoles);
    }
}
