using System.Security.Claims;
using LetsTalk.ChatService.WebApi.ChannelService;
using LetsTalk.ChatService.WebApi.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication("TestAuth")
    .AddCookie("TestAuth");
builder.Services.AddAuthorization();

//builder.Services.AddRefitClient<IChannelApiClient>()
//    .ConfigureHttpClient(http => ...);
builder.Services.AddSingleton<IChannelService, TestChannelService>();

builder.Services.AddSingleton<ConnectionManager>();

builder.Services.AddSignalR(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Automatically sign in a user for testing purposes.
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated is not true)
    {
        await context.SignInAsync("TestAuth", new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "User" + Random.Shared.Next(1000, 9999))
        ], "TestAuth")));
    }

    await next(context);
});

app.MapHub<ChatHub>("/chat")
    .RequireAuthorization();

app.Run();
