using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Services;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<User, string, UserClaim, UserLogin, UserToken>(options)
{
    public virtual DbSet<Channel> Channels { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<User>().Property(u => u.Id).HasConversion<Guid>();
        builder.Entity<UserClaim>().ToTable("UserClaims");
        builder.Entity<UserLogin>().ToTable("UserLogins");
        builder.Entity<UserToken>().ToTable("UserTokens");
    }
}
