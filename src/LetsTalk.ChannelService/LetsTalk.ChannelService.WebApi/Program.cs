using LetsTalk.ChannelService.WebApi;
using LetsTalk.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ChannelDbContext>("letstalk-channel-service-db");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddLetsTalkJwtBearer(builder.Configuration.GetRequiredSection("Jwt"));
builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapChannelEndpoints()
    .RequireAuthorization();

app.Run();
