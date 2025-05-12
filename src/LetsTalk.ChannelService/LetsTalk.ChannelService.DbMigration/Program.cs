using LetsTalk.ChannelService.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ChannelDbContext>("letstalk-channel-service",
    configureDbContextOptions: options => options.UseNpgsql(builder => builder.MigrationsAssembly(typeof(Program).Assembly)));

var app = builder.Build();

await app.StartAsync();
var dbContext = app.Services.GetRequiredService<ChannelDbContext>();
await dbContext.Database.MigrateAsync();
await app.StopAsync();
