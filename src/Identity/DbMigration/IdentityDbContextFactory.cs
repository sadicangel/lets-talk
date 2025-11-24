using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LetsTalk.Identity;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        return new IdentityDbContext(
            new DbContextOptionsBuilder<IdentityDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=identity-db;Username=postgres;Password=postgres",
                    options => options.MigrationsAssembly(typeof(Program).Assembly)).Options);
    }
}
