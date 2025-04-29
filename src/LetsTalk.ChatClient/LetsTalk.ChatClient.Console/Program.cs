using System.Net.Http.Json;
using LetsTalk.Shared.AuthModels;
using LetsTalk.Shared.Events;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHttpClient();

var app = builder.Build();

var http = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
http.BaseAddress = new Uri("https+http://letstalk-chat-service-webapi/");
http.DefaultRequestHeaders.Add("Accept", "application/json");
var userIdentity = await http.GetFromJsonAsync<LoginResponse>("/login");

// https+http://letstalk-chat-service-webapi/chat
var connection = new HubConnectionBuilder()
    .WithUrl("https+http://letstalk-chat-service-webapi/chat", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(userIdentity?.AccessToken);
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
