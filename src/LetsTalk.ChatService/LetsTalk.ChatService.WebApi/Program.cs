using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LetsTalk.ChatService.WebApi.ChannelService;
using LetsTalk.ChatService.WebApi.Services;
using LetsTalk.Shared;
using LetsTalk.Shared.AuthModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var jwtKey = new SymmetricSecurityKey(new byte[32])
{
    KeyId = Guid.NewGuid().ToString()
};

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwtKey,
        ClockSkew = TimeSpan.Zero
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

app.MapGet("/login", () =>
{
    var user = new UserIdentity(
        Guid.NewGuid().ToString(),
        "User" + Random.Shared.Next(1000, 9999),
        "https://www.gravatar.com/avatar/?d=identicon"
    );

    var token = new JwtSecurityToken(
        claims: [
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.UserData, user.AvatarUrl ?? string.Empty)
        ],
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256));


    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return TypedResults.Ok(new LoginResponse(user, tokenString, token.ValidTo));
});

app.Run();
