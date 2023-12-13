using LetsTalk;
using LetsTalk.Account;
using LetsTalk.Channels;
using LetsTalk.Services;
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
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapAccountEndpoints();
app.MapChannelEndpoints();

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

    var userId = Uuid.Create();
    var user = new User
    {
        Id = userId,
        UserName = "user@lt.com",
        NormalizedUserName = "USER@LT.COM",
        Email = "user@lt.com",
        NormalizedEmail = "USER@LT.COM",
        EmailConfirmed = false,
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

    dbContext.Users.Add(user);
    foreach (var (index, channelId) in Enumerable.Range(0, 3).Select(i => (index: i, uuid: Uuid.Create())))
    {
        dbContext.Channels.Add(new Channel
        {
            Id = channelId,
            DisplayName = $"Channel {(char)('A' + index)}",
            Icon = $"https://api.dicebear.com/7.x/shapes/svg?seed={channelId}",
            Admin = user,
            AdminId = user.Id,
            Participants = [user]
        });
    }
    dbContext.SaveChanges();
}
app.Run();
