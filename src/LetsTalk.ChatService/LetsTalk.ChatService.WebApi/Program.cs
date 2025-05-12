using LetsTalk.ChatService.WebApi.ChannelService;
using LetsTalk.ChatService.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var jwtKey = new SymmetricSecurityKey(new byte[32])
{
    KeyId = Guid.NewGuid().ToString()
};

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
            ValidateAudience = false,
            ValidAudience = builder.Configuration["Jwt:Audience"]!,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:SecurityKey"]!)),
        };
    });

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

app.MapHub<ChatHub>("/chat")
    .RequireAuthorization();

app.Run();
