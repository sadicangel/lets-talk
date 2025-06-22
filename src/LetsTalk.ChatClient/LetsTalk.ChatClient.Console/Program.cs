using LetsTalk.Shared.Events;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<CredentialsCache>();
builder.Services.AddHttpClient();

builder.Services.AddRefitClient<IIdentityService>()
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https+http://letstalk-identity-service-webapi"));
builder.Services.AddRefitClient<IChannelService>(services => new RefitSettings
{
    AuthorizationHeaderValueGetter = (_, ct) => services.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(ct)
})
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https+http://letstalk-channel-service-webapi"));

var app = builder.Build();

// Ensure registered
try
{
    await app.Services.GetRequiredService<IIdentityService>()
        .RegisterAsync(new RegisterRequest(CredentialsCache.Username, CredentialsCache.Password, CredentialsCache.Email));
}
catch (ValidationApiException)
{
    /* Ignore if user already exists */
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Ensure chats exist.
var channelService = app.Services.GetRequiredService<IChannelService>();
var response = await channelService.GetChannels();
if (response.Channels.Length == 0)
{
    await channelService.CreateChannel(new ChannelRequest("General", "General chat"));
}

var connection = new HubConnectionBuilder()
    .WithUrl("https+http://letstalk-chat-service-webapi/chat", options =>
    {
        options.AccessTokenProvider = () => app.Services.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(CancellationToken.None)!;
        options.HttpMessageHandlerFactory = _ => app.Services.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler();
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<ChannelMessage>("OnMessage", message =>
{
    logger.LogInformation("[Message] (from {@UserName}): {@Message}", message.Author.UserName, System.Text.Encoding.UTF8.GetString(message.Content));
});

connection.On<UserConnected>("OnUserConnected", message =>
{
    logger.LogInformation("[User Connected] {@UserName} (Total online: {@OnlineUsers})", message.ConnectingUser.UserName, message.OnlineUsers.Count());
});

connection.On<UserDisconnected>("OnUserDisconnected", message =>
{
    logger.LogInformation("[User Disconnected] {@UserName} (Total online: {@OnlineUsers})", message.DisconnectingUser.UserName, message.OnlineUsers.Count());
});

try
{
    logger.LogInformation("Connecting...");
    await connection.StartAsync();
    logger.LogInformation("Connected!");

    // Simple loop to send messages
    //while (true)
    //{
    //    logger.Write("Enter channel ID: ");
    //    var channelId = logger.ReadLine()!;
    //    logger.Write("Enter message: ");
    //    var msg = logger.ReadLine()!;

    var content = System.Text.Encoding.UTF8.GetBytes("Test message");
    await connection.InvokeAsync("SendChannelMessage", "1", "text/plain", content);
    //}
}
catch (Exception ex)
{
    logger.LogError(ex, "Connection failed");
}

sealed class CredentialsCache(IServiceProvider serviceProvider)
{
    public const string Username = "test";
    public const string Password = "Test1234!";
    public const string Email = "test@letstalk.com";

    private AccessTokenResponse? _response;
    private DateTimeOffset _expires;

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        if (_response is null)
        {
            _response = await serviceProvider.GetRequiredService<IIdentityService>()
                .LoginAsync(new LoginRequest(Username, Password), cancellationToken);
            _expires = DateTimeOffset.UtcNow.AddSeconds(_response.ExpiresIn - 30);
        }
        else if (_expires <= DateTimeOffset.UtcNow)
        {
            _response = await serviceProvider.GetRequiredService<IIdentityService>()
                .RefreshAsync(new RefreshRequest(_response.RefreshToken), cancellationToken);
            _expires = DateTimeOffset.UtcNow.AddSeconds(_response.ExpiresIn - 30);

            return _response.AccessToken;
        }

        return _response.AccessToken;
    }
}
