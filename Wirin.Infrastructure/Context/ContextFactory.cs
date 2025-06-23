using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Wirin.Infrastructure.Context;

public class WirinDbContextFactory : IDesignTimeDbContextFactory<WirinDbContext>
{
    public WirinDbContext CreateDbContext(string[] args)
    {
        // Load configuration (e.g., from appsettings.json)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // This might need to be adjusted based on the location
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<WirinDbContext>();

        var connectionString = configuration["ConnectionStrings:DefaultConnection"];

        optionsBuilder.UseSqlServer(connectionString);

        return new WirinDbContext(optionsBuilder.Options);
    }
}


