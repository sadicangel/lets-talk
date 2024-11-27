using Bogus;
using LetsTalk.WebApi.Endpoints;
using LetsTalk.WebApi.Entities;
using LetsTalk.WebApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<LetsTalkDbContext>("letstalk-database",
    configureDbContextOptions: opts => opts
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());
builder.EnrichNpgsqlDbContext<LetsTalkDbContext>();
builder.Services.AddPooledDbContextFactory<LetsTalkDbContext>(opts => { });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddSingleton<MessageBatchService>();
builder.Services.AddHostedService<MessageBatchService>();
builder.Services.AddSingleton<ConnectionManager>();

builder.Services.AddOpenApi();
builder.Services.AddSignalR(opts => opts.EnableDetailedErrors = true);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var serviceScope = app.Services.CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<LetsTalkDbContext>();
    var password = serviceScope.ServiceProvider.GetRequiredService<PasswordHasher>().HashPassword(null!, "password");
    dbContext.Database.EnsureCreated();
    dbContext.Users.AddRange(new Faker<User>()
        .RuleFor(u => u.Id, f => Guid.CreateVersion7().ToString())
        .RuleFor(u => u.UserName, f => f.Person.UserName)
        .RuleFor(u => u.Email, f => f.Person.Email)
        .RuleFor(u => u.PasswordHash, password)
        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(u => u.LastSeenAt, f => f.Date.PastOffset())
        .Generate(10));
    dbContext.Channels.Add(new Channel
    {
        Id = "019341b2-ff36-7798-ad0b-cb7ecc9fb128",
        DisplayName = "General",
        IconUrl = default,
        AdminId = dbContext.Users.Local.First().Id,
        Members = [.. dbContext.Users.Local],
    });
    dbContext.SaveChanges();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<LetsTalkHub>("/hub")
    .RequireAuthorization();

app.MapAuthEndpoints();

app.Run();
