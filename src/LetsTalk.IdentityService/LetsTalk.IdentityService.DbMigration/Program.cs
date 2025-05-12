using LetsTalk.IdentityService.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<IdentityDbContext>("letstalk-identity-service",
    configureDbContextOptions: options => options.UseNpgsql(builder => builder.MigrationsAssembly(typeof(Program).Assembly)));

var app = builder.Build();

await app.StartAsync();
var dbContext = app.Services.GetRequiredService<IdentityDbContext>();
await dbContext.Database.MigrateAsync();
await app.StopAsync();

