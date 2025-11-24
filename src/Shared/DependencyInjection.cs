using LetsTalk.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Refit;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

public static class LetsTalkExtensions
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddCredentialsCache(IConfiguration configuration)
        {
            services.TryAddSingleton<CredentialsCache>();
            services.Configure<CredentialsCacheOptions>(configuration.GetRequiredSection(CredentialsCacheOptions.SectionName));

            return services;
        }

        public IServiceCollection AddIdentityApiClient(IConfiguration configuration)
        {
            services.Configure<IdentityApiOptions>(configuration.GetRequiredSection(IdentityApiOptions.SectionName));
            services.AddRefitClient<IIdentityApi>(services => new RefitSettings { AuthorizationHeaderValueGetter = (_, ct) => services.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(ct) })
                .ConfigureHttpClient((provider, http) =>
                {
                    http.BaseAddress = new Uri(provider.GetRequiredService<IOptions<IdentityApiOptions>>().Value.Url);
                });

            return services;
        }

        public IServiceCollection AddChatApiClient(IConfiguration configuration)
        {
            services.AddCredentialsCache(configuration);
            services.Configure<ChatApiOptions>(configuration.GetRequiredSection(ChatApiOptions.SectionName));
            services.AddRefitClient<IChatApi>(services => new RefitSettings { AuthorizationHeaderValueGetter = (_, ct) => services.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(ct) })
                .ConfigureHttpClient((provider, http) => http.BaseAddress = new Uri(provider.GetRequiredService<IOptions<ChatApiOptions>>().Value.Url));
            return services;
        }

        public IServiceCollection AddChatHubClient<TChatHub>(IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TChatHub : ChatHubClient
        {
            services.AddCredentialsCache(configuration);
            services.Configure<ChatHubOptions>(configuration.GetRequiredSection(ChatHubOptions.SectionName));
            services.Add(
                new ServiceDescriptor(
                    serviceType: typeof(HubConnection),
                    factory: provider => new HubConnectionBuilder()
                        .WithUrl(
                            provider.GetRequiredService<IOptions<ChatHubOptions>>().Value.Url,
                            options =>
                            {
                                options.AccessTokenProvider = () => provider.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(CancellationToken.None)!;
                                options.HttpMessageHandlerFactory = _ => provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler();
                            })
                        .WithAutomaticReconnect()
                        .Build(),
                    lifetime: serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(ChatHubClient), typeof(TChatHub), serviceLifetime));
            return services;
        }
    }

    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddLetsTalkJwtBearer(IConfiguration configuration)
        {
            const string SectionPath = "LetsTalk:Jwt";
            var options = configuration.GetRequiredSection(SectionPath).Get<LetsTalkJwtOptions>()
                ?? throw new InvalidOperationException($"Missing config section '{SectionPath}'");

            return builder.AddJwtBearer(x =>
            {
                x.MapInboundClaims = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = false,
                    ValidAudience = options.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(options.SecurityKey)),
                };
            });
        }
    }

    private sealed record class LetsTalkJwtOptions(string Issuer, string Audience, string SecurityKey);
}
