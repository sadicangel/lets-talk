using LetsTalk;
using LetsTalk.Account;
using LetsTalk.Channels;
using LetsTalk.Events;
using LetsTalk.Services;
using Microsoft.AspNetCore.SignalR;
using Polly;

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
    app.MapGet("test/notification", async (string message, IHubContext<AppHub, IAppHubClient> hubContext) =>
    {
        await hubContext.Clients.All.OnNotificationBroadcast(new NotificationBroadcast(
            message,
            DateTimeOffset.UtcNow
        ));

        return TypedResults.Ok();
    });

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAccountEndpoints();
app.MapChannelEndpoints();

app.MapHub<AppHub>("/letstalk")
    .RequireAuthorization();

{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Policy.Handle<Exception>()
        .WaitAndRetry(10, i => TimeSpan.FromMilliseconds(500 * i))
        .Execute(() =>
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var adminId = Uuid.Create();
            var admin = new User
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@lt.com",
                NormalizedEmail = "ADMIN@LT.COM",
                EmailConfirmed = true,
                Avatar = $"https://api.dicebear.com/7.x/fun-emoji/svg?seed={adminId}",
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

            var userId = Uuid.Create();
            var user = new User
            {
                Id = userId,
                UserName = "user",
                NormalizedUserName = "USER",
                Email = "user@lt.com",
                NormalizedEmail = "USER@LT.COM",
                EmailConfirmed = true,
                Avatar = $"https://api.dicebear.com/7.x/fun-emoji/svg?seed={userId}",
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

            dbContext.Users.Add(admin);
            dbContext.Users.Add(user);
            foreach (var (index, channelId) in Enumerable.Range(0, 3).Select(i => (index: i, uuid: Uuid.Create())))
            {
                dbContext.Channels.Add(new Channel
                {
                    Id = channelId,
                    DisplayName = $"Channel {(char)('A' + index)}",
                    Icon = $"https://api.dicebear.com/7.x/shapes/svg?seed={channelId}",
                    Admin = admin,
                    AdminId = admin.Id,
                    Participants = [admin, user]
                });
            }
            dbContext.SaveChanges();
        });
}
app.Run();
