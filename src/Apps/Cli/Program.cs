using LetsTalk.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHttpClient();
builder.Services.AddIdentityApiClient(builder.Configuration);
builder.Services.AddChatApiClient(builder.Configuration);
builder.Services.AddChatHubClient<ChatHubClient>(builder.Configuration);

var app = builder.Build();

var identityApi = app.Services.GetRequiredService<IIdentityApi>();

// Ensure registered
try
{
    var options = app.Services.GetRequiredService<IOptions<CredentialsCacheOptions>>().Value;
    await identityApi
        .RegisterAsync(
            new RegisterRequest(
                UserName: options.Username,
                Password: options.Password,
                Email: $"{options.Username.ToLowerInvariant()}@email.com",
                AvatarUrl: "https://i.pravatar.cc/300?img=3"));
}
catch (ValidationApiException)
{
    /* Ignore if user already exists */
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Log current profile
var profile = await identityApi.GetProfileAsync();
logger.LogInformation("Logged in as '{UserName}' ({Email})", profile.UserName, profile.Email);

// Ensure chats exist.
var chatService = app.Services.GetRequiredService<IChatApi>();
var response = await chatService.GetChannels();
var channel = response.Channels.FirstOrDefault()
    ?? await chatService.CreateChannel(new ChannelRequest("General", "General chat"));


var client = app.Services.GetRequiredService<ChatHubClient>();

try
{
    logger.LogInformation("Connecting...");
    await client.ConnectAsync();
    logger.LogInformation("Connected!");

    // Simple loop to send messages
    //while (true)
    //{
    //    logger.Write("Enter channel ID: ");
    //    var channelId = logger.ReadLine()!;
    //    logger.Write("Enter message: ");
    //    var msg = logger.ReadLine()!;

    await client.SendMessage(channel.ChannelId, contentType: "text/plain", content: "Test message"u8.ToArray());
    //}
}
catch (Exception ex)
{
    logger.LogError(ex, "Connection failed");
}
