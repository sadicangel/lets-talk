using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LetsTalk.Chat;

public sealed class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        return new ChatDbContext(
            new DbContextOptionsBuilder<ChatDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=chat-db;Username=postgres;Password=postgres",
                    options => options.MigrationsAssembly(typeof(Program).Assembly)).Options)
        {
            Channels = null!,
            Members = null!,
            Messages = null!
        };
    }
}
