using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Services;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<User, string, UserClaim, UserLogin, UserToken>(options)
{
    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<User>().Property(u => u.Id).HasConversion<Guid>();
        builder.Entity<UserClaim>().ToTable("UserClaims");
        builder.Entity<UserLogin>().ToTable("UserLogins");
        builder.Entity<UserToken>().ToTable("UserTokens");

        builder.Entity<User>(b =>
        {
            b.HasMany(u => u.Channels)
            .WithMany(c => c.Participants)
            .UsingEntity<UserChannel>();
        });

        builder.Entity<Channel>(b =>
        {
            b.HasOne(x => x.Admin)
                .WithMany()
                .HasForeignKey(x => x.AdminId);
        });

        builder.Entity<UserChannel>(b =>
        {
            b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);

            b.HasOne(x => x.Channel)
            .WithMany()
            .HasForeignKey(x => x.ChannelId);

            b.Property(x => x.MemberSince)
            .HasDefaultValueSql("now()");
        });

        builder.Entity<Message>(b =>
        {
            b.HasOne(x => x.Channel)
                .WithMany(x => x.Messages);

            b.HasOne(x => x.Sender)
                .WithMany();

            b.Property(x => x.Timestamp)
            .HasDefaultValueSql("now()");
        });
    }
}
