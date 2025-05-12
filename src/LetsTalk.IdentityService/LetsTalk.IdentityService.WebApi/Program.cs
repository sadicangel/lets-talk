using LetsTalk.IdentityService.WebApi;
using LetsTalk.IdentityService.WebApi.Endpoints;
using LetsTalk.IdentityService.WebApi.Services;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<IdentityDbContext>("letstalk-identity-service");

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();

builder.Services.AddSingleton<JwtBearerTokenProvider>();
builder.Services.Configure<JwtBearerTokenProviderOptions>(builder.Configuration.GetRequiredSection("Jwt"))
    .AddOptionsWithValidateOnStart<JwtBearerTokenProviderOptions>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.MapIdentityEndpoints();

app.Run();
