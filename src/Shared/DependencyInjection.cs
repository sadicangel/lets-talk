using LetsTalk.Configuration;
using LetsTalk.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        public IServiceCollection AddChatApiClient<TChatHub>(IConfiguration configuration)
            where TChatHub : ChatHubClient
        {
            services.AddCredentialsCache(configuration);
            services.Configure<ChatApiOptions>(configuration.GetRequiredSection(ChatApiOptions.SectionName));
            services.AddRefitClient<IChatApi>(services => new RefitSettings { AuthorizationHeaderValueGetter = (_, ct) => services.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(ct) })
                .ConfigureHttpClient((provider, http) => http.BaseAddress = new Uri(provider.GetRequiredService<IOptions<ChatApiOptions>>().Value.Url));
            services.Add(
                new ServiceDescriptor(
                    serviceType: typeof(HubConnection),
                    factory: provider =>
                    {
                        var chatApi = provider.GetRequiredService<IOptions<ChatApiOptions>>().Value;
                        return new HubConnectionBuilder()
                            .WithUrl(
                                new Uri(chatApi.Url) + chatApi.HubName,
                                options =>
                                {
                                    options.AccessTokenProvider = () => provider.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(CancellationToken.None)!;
                                    options.HttpMessageHandlerFactory = _ => provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler();
                                })
                            .WithAutomaticReconnect()
                            .Build();
                    },
                    lifetime: ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ChatHubClient), typeof(TChatHub), ServiceLifetime.Transient));
            return services;
        }
    }

    extension(AuthenticationBuilder builder)
    {
        public AuthenticationBuilder AddLetsTalkJwtBearer(IConfiguration configuration)
        {
            builder.Services.Configure<LetsTalkJwtOptions>(configuration.GetRequiredSection(LetsTalkJwtOptions.SectionName));
            builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<LetsTalkJwtOptions>>((bearerOptions, sectionOptions) =>
                {
                    bearerOptions.MapInboundClaims = false;
                    bearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = sectionOptions.Value.Issuer,
                        ValidateAudience = false,
                        ValidAudience = sectionOptions.Value.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(sectionOptions.Value.SecurityKey)),
                    };
                    bearerOptions.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"].ToString();
                            if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hub"))
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        }
                    };
                });

            return builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
        }
    }
}
