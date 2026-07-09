using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RentAHome.Persistence;

public sealed class RentAHomeDbContextFactory : IDesignTimeDbContextFactory<RentAHomeDbContext>
{
    public RentAHomeDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("RENTAHOME_DATABASE")
            ?? "Host=localhost;Port=5432;Database=rentahome_dev;Username=rentahome;Password=rentahome_dev_password";

        var optionsBuilder = new DbContextOptionsBuilder<RentAHomeDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsAssembly(typeof(RentAHomeDbContext).Assembly.FullName));

        return new RentAHomeDbContext(optionsBuilder.Options);
    }
}
