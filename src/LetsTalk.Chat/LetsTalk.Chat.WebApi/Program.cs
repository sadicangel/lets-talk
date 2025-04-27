using System.Security.Claims;
using LetsTalk.Chat.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services.AddAuthentication("TestAuth")
    .AddCookie("TestAuth");
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();
builder.Services.AddSignalR(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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

app.MapGet("/user", (HttpContext context) => new
{
    Id = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
    context.User.Identity?.Name
});

app.MapHub<ChatHub>("/chat", options =>
{
});

app.Run();
