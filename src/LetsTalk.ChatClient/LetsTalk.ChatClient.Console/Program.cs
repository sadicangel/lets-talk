using LetsTalk.Shared.Events;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    Console.WriteLine($"[Message] {message.UserId}: {System.Text.Encoding.UTF8.GetString(message.Content)}");
});

connection.On<UserConnected>("OnUserConnected", payload =>
{
    Console.WriteLine($"[User Connected] {payload.ConnectingUser.UserName} (Total online: {payload.Users.Count()})");
});

connection.On<UserDisconnected>("OnUserDisconnected", payload =>
{
    Console.WriteLine($"[User Disconnected] {payload.DisconnectingUser.UserName} (Total online: {payload.Users.Count()})");
});

try
{
    Console.WriteLine("Connecting...");
    await connection.StartAsync();
    Console.WriteLine("Connected!");

    // Simple loop to send messages
    //while (true)
    //{
    //    Console.Write("Enter channel ID: ");
    //    var channelId = Console.ReadLine()!;
    //    Console.Write("Enter message: ");
    //    var msg = Console.ReadLine()!;

    var content = System.Text.Encoding.UTF8.GetBytes("Test message");
    await connection.InvokeAsync("SendChannelMessage", "1", "text/plain", content);
    //}
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
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
