using System.Runtime.InteropServices;
using System.Security.Claims;
using Bogus;
using LetsTalk.WebApi.Entities;
using LetsTalk.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<LetsTalkDbContext>("letstalk-database",
    configureDbContextOptions: opts => opts
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());
builder.EnrichNpgsqlDbContext<LetsTalkDbContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddOpenApi();
builder.Services.AddSignalR(opts => opts.EnableDetailedErrors = true);
builder.Services.AddSingleton<ConnectionManager>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var serviceScope = app.Services.CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<LetsTalkDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Users.AddRange(new Faker<User>()
        .RuleFor(u => u.Id, f => Guid.CreateVersion7())
        .RuleFor(u => u.UserName, f => f.Person.UserName)
        .RuleFor(u => u.Email, f => f.Person.Email)
        .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
        .RuleFor(u => u.PasswordSalt, f => f.Internet.Password())
        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastSeenAt, f => f.Date.PastOffset())
        .Generate(10));
    dbContext.Channels.Add(new Channel
    {
        Id = Guid.Parse("019341b2-ff36-7798-ad0b-cb7ecc9fb128"),
        DisplayName = "General",
        IconUrl = default,
        AdminId = dbContext.Users.Local.First().Id,
        //Members = [.. dbContext.Users.Local],
    });
    dbContext.SaveChanges();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.Use(static async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated is not true)
    {
        using var dbContext = context.RequestServices.GetRequiredService<LetsTalkDbContext>();
        var users = dbContext.Users.ToList();
        Random.Shared.Shuffle(CollectionsMarshal.AsSpan(users));
        var user = users[0];

        var claimsIdentity = new ClaimsIdentity(
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    }
    await next.Invoke();
});

app.MapHub<LetsTalkHub>("/lets-talk");

app.MapFallbackToFile("index.html");

app.Run();
