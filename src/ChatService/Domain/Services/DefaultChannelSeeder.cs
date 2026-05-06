using LetsTalk.Chat.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Services;

public sealed class DefaultChannelSeeder(IServiceScopeFactory scopeFactory, ILogger<DefaultChannelSeeder> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

        if (await dbContext.Channels.AnyAsync(stoppingToken))
            return;

        dbContext.Channels.Add(
            new Channel
            {
                Id = null!,
                Name = "General",
                Description = "General chat",
                Members = [],
                Messages = []
            });

        await dbContext.SaveChangesAsync(stoppingToken);
        logger.LogInformation("Seeded default General channel.");
    }
}
