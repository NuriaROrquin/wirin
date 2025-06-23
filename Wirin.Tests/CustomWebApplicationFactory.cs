using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Seeders;

namespace Wirin.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    static CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("FRONT_URL", "http://localhost");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Eliminá el DbContext original (SQL Server)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<WirinDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Reemplazalo por uno InMemory
            services.AddDbContext<WirinDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestWirinDb");
            });

            // Forzá creación de la BD
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scope.ServiceProvider.GetRequiredService<WirinDbContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<UserEntity>>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

            db.Database.EnsureCreated();

            IdentitySeeder.SeedRolesAsync(scopedServices).Wait();
            IdentitySeeder.SeedUsersAsync(scopedServices).Wait();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Usá entorno de desarrollo por defecto
        builder.UseEnvironment("Development");
        return base.CreateHost(builder);
    }
}
