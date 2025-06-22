using LetsTalk.ChatClient.Console.Services;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<CredentialsCache>()
    .Configure<CredentialsCacheOptions>(builder.Configuration.GetRequiredSection(CredentialsCacheOptions.SectionName));

builder.Services.AddSingleton<HubConnection>(provider => new HubConnectionBuilder()
    .WithUrl("https+http://letstalk-chat-service-webapi/chat", options =>
    {
        options.AccessTokenProvider = () => provider.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(CancellationToken.None)!;
        options.HttpMessageHandlerFactory = _ => provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler();
    })
    .WithAutomaticReconnect()
    .Build());

builder.Services.AddSingleton<LetsTalkClient, LoggingLetsTalkClient>();

builder.Services.AddRefitClient<IIdentityService>()
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https+http://letstalk-identity-service-webapi"));
builder.Services.AddRefitClient<IChatService>(services => new RefitSettings
{
    AuthorizationHeaderValueGetter = (_, ct) => services.GetRequiredService<CredentialsCache>().GetBearerTokenAsync(ct)
})
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https+http://letstalk-chat-service-webapi"));

var app = builder.Build();

// Ensure registered
try
{
    var options = app.Services.GetRequiredService<IOptions<CredentialsCacheOptions>>().Value;
    await app.Services.GetRequiredService<IIdentityService>()
        .RegisterAsync(new RegisterRequest(options.Username, options.Password, options.Email));
}
catch (ValidationApiException)
{
    /* Ignore if user already exists */
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Ensure chats exist.
var chatService = app.Services.GetRequiredService<IChatService>();
var response = await chatService.GetChannels();
var channelId = response.Channels.FirstOrDefault();
if (channelId is null)
{
    var channel = await chatService.CreateChannel(new ChannelRequest("General", "General chat"));
    channelId = channel.ChannelId;
}


var client = app.Services.GetRequiredService<LetsTalkClient>();

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

    await client.SendChannelMessage(channelId, contentType: "text/plain", content: System.Text.Encoding.UTF8.GetBytes("Test message"));
    //}
}
catch (Exception ex)
{
    logger.LogError(ex, "Connection failed");
}
