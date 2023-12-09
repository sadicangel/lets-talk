using LetsTalk;
using LetsTalk.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("postgres");

builder.AddRedisDistributedCache("redis");

builder.Services.AddDomain();

builder.Services.AddAuthorization();

builder.Services
    .AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

var account = app.MapGroup("/account").MapIdentityApi<User>();

app.MapGet("/account/profile", async (UserManager<User> manager, AppDbContext dbContext, ClaimsPrincipal principal) =>
{
    var user = await manager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    if (user is null)
        return Results.NotFound();

    await dbContext.Entry(user).Collection(x => x.Channels).LoadAsync();

    return Results.Ok(new UserProfile(user));
}).RequireAuthorization();

app.MapHub<AppHub>("/letstalk");

{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Policy.Handle<Exception>()
        .WaitAndRetry(10, i => TimeSpan.FromMilliseconds(500 * i))
        .Execute(() =>
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });

    var user = new User
    {
        Id = "e4e370a1-8a46-4323-80f5-a5e6b1e91d5e",
        UserName = "user@lt.com",
        NormalizedUserName = "USER@LT.COM",
        Email = "user@lt.com",
        NormalizedEmail = "USER@LT.COM",
        EmailConfirmed = false,
        Avatar = "https://api.dicebear.com/7.x/fun-emoji/svg?seed=e4e370a1-8a46-4323-80f5-a5e6b1e91d5e",
        PasswordHash = "AQAAAAIAAYagAAAAENlO6EaetzL0KAJCSHYObRTLwxD/XFsstNC1JGBtdV4TMpghkn7dEscPdoRR83bO/g==",
        SecurityStamp = "BVZ3RAYR2STMNMXPUOT5KPA5OAH27OYW",
        ConcurrencyStamp = "b610bb71-712e-4c8d-b0d7-cb4dddee1e97",
        PhoneNumber = null,
        PhoneNumberConfirmed = false,
        TwoFactorEnabled = false,
        LockoutEnd = null,
        LockoutEnabled = true,
        AccessFailedCount = 0
    };

    dbContext.Users.Add(user);
    var channelId = Guid.NewGuid().ToString();
    dbContext.Channels.Add(new Channel
    {
        Id = "6e5312d6-57e0-4362-925d-a3881c1e5df7",
        DisplayName = "admins",
        Icon = "https://api.dicebear.com/7.x/shapes/svg?seed=6e5312d6-57e0-4362-925d-a3881c1e5df7",
        Admin = user,
        Participants = [user]
    });
    dbContext.SaveChanges();
    var channels = dbContext.Channels.Include(c => c.Participants).ToList();
}
app.Run();
