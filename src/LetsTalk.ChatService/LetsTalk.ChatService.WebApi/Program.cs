using LetsTalk.ChatService.Domain;
using LetsTalk.ChatService.WebApi.Channels;
using LetsTalk.ChatService.WebApi.Hubs;
using LetsTalk.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var jwtKey = new SymmetricSecurityKey(new byte[32])
{
    KeyId = Guid.NewGuid().ToString()
};

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddMassTransitRabbitMq("letstalk-rabbitmq");

builder.AddNpgsqlDbContext<ChatDbContext>("letstalk-chat-service-db");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddLetsTalkJwtBearer(builder.Configuration.GetRequiredSection("Jwt"));
builder.Services.AddAuthorization();

builder.Services.AddSingleton<ConnectionManager>();

builder.Services.AddSignalR(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapChannelEndpoints().RequireAuthorization();
app.MapHub<ChatHub>("/chat").RequireAuthorization();

app.Run();
