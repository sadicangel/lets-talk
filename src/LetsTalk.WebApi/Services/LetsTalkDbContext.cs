﻿using LetsTalk.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.WebApi.Services;

public class LetsTalkDbContext(DbContextOptions<LetsTalkDbContext> options)
    : DbContext(options)
{
    public virtual required DbSet<User> Users { get; init; }
    public virtual required DbSet<Channel> Channels { get; init; }
    public virtual required DbSet<Message> Messages { get; init; }
    public virtual required DbSet<Notification> Notifications { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Channels)
            .WithMany(c => c.Members)
            .UsingEntity<ChannelMember>(builder => builder
                .Property(cm => cm.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"));

        modelBuilder.Entity<User>()
            .HasMany(u => u.Messages)
            .WithOne(m => m.Author);

        modelBuilder.Entity<Channel>()
            .HasOne(c => c.Admin)
            .WithMany();

        modelBuilder.Entity<Channel>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Channel);
    }
}
