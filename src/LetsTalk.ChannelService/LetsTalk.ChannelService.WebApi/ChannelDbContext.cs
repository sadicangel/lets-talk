using Microsoft.EntityFrameworkCore;

namespace LetsTalk.ChannelService.WebApi;

public class ChannelDbContext(DbContextOptions<ChannelDbContext> options) : DbContext(options)
{
    public required virtual DbSet<Channel> Channels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Channel>(channel =>
        {
            channel.HasKey(c => c.Id);
            channel.Property(c => c.Name).IsRequired();
        });

        modelBuilder.Entity<ChannelMember>(member =>
        {
            member.HasKey(m => new { m.ChannelId, m.UserId }); // Composite key
            member
                .HasOne(m => m.Channel)
                .WithMany(c => c.Members)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            member.Property(m => m.MemberSince).IsRequired();
            member.Property(m => m.LastSeenAt).IsRequired();
            member.Property(m => m.Role).HasConversion<string>().IsRequired();
            member.Property(m => m.Status).HasConversion<string>().IsRequired();
        });
    }
}

public sealed class Channel
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<ChannelMember> Members { get; init; } = default!;
}

public enum ChannelRole
{
    Member,
    Moderator,
    Admin
}
public enum ChannelMembershipStatus
{
    Active,
    Muted,
    Banned,
    Invited,
    Pending,
}

public sealed class ChannelMember
{
    public required string ChannelId { get; init; }
    public Channel Channel { get; init; } = null!;
    public required string UserId { get; init; }
    public required DateTimeOffset MemberSince { get; init; }
    public required DateTimeOffset LastSeenAt { get; set; }
    public ChannelRole Role { get; set; }
    public ChannelMembershipStatus Status { get; set; }
    public string? InvitedByUserId { get; set; }
}
