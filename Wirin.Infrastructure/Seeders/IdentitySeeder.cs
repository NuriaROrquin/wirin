using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();

        string[] roles = { "Bibliotecario", "Voluntario", "Voluntario Administrativo", "Admin", "Alumno" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<string> { Id = Guid.NewGuid().ToString(), Name = role });
            }
        }
    }

    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

        var users = new List<(string FullName, string Email, string Password, string PhoneNumber, string[] Roles)>
        {
            // Admin
            ("Maria Gonzalez", "mariagonzalez@biblioteca.com", "Test123.", "123-456-7890", new[] { "Bibliotecario", "Admin" }),
    
            // Voluntarios
            ("Lionel Pares", "lionelpares@biblioteca.com", "Test123.", "234-567-8901", new[] { "Voluntario" }),
            ("Martina López", "martinalopez@biblioteca.com", "Test123.", "345-678-9012", new[] { "Voluntario" }),
            ("Javier Torres", "javiertorres@biblioteca.com", "Test123.", "456-789-0123", new[] { "Voluntario" }),

            // Voluntarios Administrativos
            ("Jose Ruiz", "joseruiz@biblioteca.com", "Test123.", "567-890-1234", new[] { "Voluntario Administrativo" }),
            ("Laura Castillo", "lauracastillo@biblioteca.com", "Test123.", "678-901-2345", new[] { "Voluntario Administrativo" }),
            ("Rodrigo Méndez", "rodrigomendez@biblioteca.com", "Test123.", "789-012-3456", new[] { "Voluntario Administrativo" }),

            // Alumnos
            ("Juan Perez", "juanperez@biblioteca.com", "Test123.", "890-123-4567", new[] { "Alumno" }),
            ("Carlos Rodríguez", "carlosrodriguez@biblioteca.com", "Test123.", "901-234-5678", new[] { "Alumno" }),
            ("Ana Fernández", "anafernandez@biblioteca.com", "Test123.", "012-345-6789", new[] { "Alumno" }),
            ("Sofía López", "sofialopez@biblioteca.com", "Test123.", "123-456-7890", new[] { "Alumno" })
        };

        foreach (var (fullName, email, password, phoneNumber, roles) in users)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var newUser = new UserEntity
                {
                    FullName = fullName,
                    UserName = email,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    EmailConfirmed = true
                };


                var result = await userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(newUser, roles.ToArray());
                }
            }
        }
    }
}