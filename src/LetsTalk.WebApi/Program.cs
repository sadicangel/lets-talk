using System.Security.Claims;
using LetsTalk.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.Use(static async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated is not true)
    {
        var claimsIdentity = new ClaimsIdentity(
            [
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    }
    await next.Invoke();
});

app.MapHub<LetsTalkHub>("/lets-talk");

app.MapFallbackToFile("index.html");

app.Run();
