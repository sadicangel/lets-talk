using LetsTalk.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.WebApi.Services;

public class LetsTalkDbContext(DbContextOptions<LetsTalkDbContext> options)
    : DbContext(options)
{
    public virtual required DbSet<User> Users { get; init; }
    public virtual required DbSet<Channel> Channels { get; init; }
    public virtual required DbSet<Message> Messages { get; init; }
    public virtual required DbSet<Notification> Notifications { get; init; }
}
