using LetsTalk;
using LetsTalk.Services;
using Microsoft.AspNetCore.Identity;
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

var account = app.MapGroup("/account");
account.MapIdentityApi<User>();
account.MapGet("/profile", async (UserManager<User> manager, ClaimsPrincipal principal) =>
{
    var user = await manager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    return user is not null
        ? Results.Ok(new UserProfile(user))
        : Results.NotFound();
}).RequireAuthorization();

app.MapHub<AppHub>("/letstalk");

{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
