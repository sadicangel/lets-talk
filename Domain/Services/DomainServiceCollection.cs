using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Services;
public static class DomainServiceCollection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<HubConnectionManager>();
        return services;
    }
}
