using LetsTalk.Chat;
using LetsTalk.Chat.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton(TimeProvider.System)
            .AddDbContextPool<ChatDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("chat-db")))
            .AddSingleton<ConnectionManager>();

        return services;
    }
}
