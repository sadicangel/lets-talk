using LetsTalk.ChatService.WebApi.Services;
using LetsTalk.Shared;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Refit;

var jwtKey = new SymmetricSecurityKey(new byte[32])
{
    KeyId = Guid.NewGuid().ToString()
};

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddLetsTalkJwtBearer(builder.Configuration.GetRequiredSection("Jwt"));
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddRefitClient<IChannelService>(provider => new RefitSettings
{
    AuthorizationHeaderValueGetter = (_, ct) => Task.FromResult(provider
        .GetRequiredService<IHttpContextAccessor>()
        .HttpContext?.Request.Headers[HeaderNames.Authorization]
        .FirstOrDefault()?.Replace("Bearer ", "") ?? "")
})
    .ConfigureHttpClient(http => http.BaseAddress = new Uri("https://letstalk-channel-service-webapi"));

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
