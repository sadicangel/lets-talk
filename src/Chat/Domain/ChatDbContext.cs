using LetsTalk.Chat.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace LetsTalk.Chat;

public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public virtual required DbSet<Channel> Channels { get; set; }
    public virtual required DbSet<ChannelMember> Members { get; set; }
    public virtual required DbSet<ChannelMessage> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Channel>(channel =>
        {
            channel.HasKey(c => c.Id);
            channel.Property(c => c.Id)
                .HasValueGenerator<GuidV7ValueGenerator>()
                .ValueGeneratedOnAdd();
            channel.Property(c => c.Version).IsRowVersion();
            channel.Property(c => c.Name).IsRequired().HasMaxLength(128);
            channel.Property(c => c.Description).HasMaxLength(128);
        });

        modelBuilder.Entity<ChannelMember>(member =>
        {
            member.HasKey(m => new
            {
                m.ChannelId,
                m.UserId
            });
            member
                .HasOne(m => m.Channel)
                .WithMany(c => c.Members)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            member.Property(m => m.ChannelId).HasMaxLength(36);
            member.Property(m => m.UserId).HasMaxLength(36);
            member.Property(c => c.Version).IsRowVersion();
            member.Property(m => m.MemberSince).IsRequired();
            member.Property(m => m.LastSeenAt).IsRequired();
            member.Property(m => m.Role).HasConversion<string>().IsRequired();
            member.Property(m => m.Status).HasConversion<string>().IsRequired();
            member.Property(m => m.InvitedByUserId).HasMaxLength(36);
        });

        modelBuilder.Entity<ChannelMessage>(message =>
        {
            message.HasKey(m => m.Id);
            message.Property(c => c.Id)
                .HasValueGenerator<GuidV7ValueGenerator>()
                .ValueGeneratedOnAdd();
            message.Property(c => c.Version).IsRowVersion();
            message.HasOne(m => m.Channel)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            message.Property(m => m.ChannelId).HasMaxLength(36);
            message.Property(m => m.AuthorId).IsRequired().HasMaxLength(36);
            message.Property(m => m.ContentType).IsRequired().HasMaxLength(128);
            message.Property(m => m.Content).IsRequired();
            message.Property(m => m.Timestamp).IsRequired();
        });
    }
}

public class GuidV7ValueGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => Guid.CreateVersion7().ToString();
}
