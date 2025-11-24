using LetsTalk.Identity;
using LetsTalk.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDomain(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddLetsTalkJwtBearer(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddSingleton<JwtBearerTokenProvider>();
builder.Services.Configure<JwtBearerTokenProviderOptions>(builder.Configuration.GetSection("LetsTalk:Jwt"))
    .AddOptionsWithValidateOnStart<JwtBearerTokenProviderOptions>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityEndpoints();
app.MapProfileEndpoints();

app.Run();
