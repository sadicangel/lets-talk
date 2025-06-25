using LetsTalk.ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace LetsTalk.ChatService.Domain;

public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public required virtual DbSet<Channel> Channels { get; set; }
    public required virtual DbSet<ChannelMember> Members { get; set; }
    public required virtual DbSet<ChannelMessage> Messages { get; set; }

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
            channel.Property(c => c.Name).IsRequired();
        });

        modelBuilder.Entity<ChannelMember>(member =>
        {
            member.HasKey(m => new { m.ChannelId, m.UserId });
            member
                .HasOne(m => m.Channel)
                .WithMany(c => c.Members)
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
            member.Property(c => c.Version).IsRowVersion();

            member.Property(m => m.MemberSince).IsRequired();
            member.Property(m => m.LastSeenAt).IsRequired();
            member.Property(m => m.Role).HasConversion<string>().IsRequired();
            member.Property(m => m.Status).HasConversion<string>().IsRequired();
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

            message.Property(m => m.AuthorId).IsRequired();
            message.Property(m => m.ContentType).IsRequired();
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
