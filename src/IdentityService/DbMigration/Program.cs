using LetsTalk.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(
    connectionString: builder.Configuration.GetConnectionString("identity-db"),
    npgsqlOptionsAction: x => x.MigrationsAssembly(typeof(Program).Assembly)));

var app = builder.Build();

await app.StartAsync();
var dbContext = app.Services.GetRequiredService<IdentityDbContext>();
await dbContext.Database.MigrateAsync();
await app.StopAsync();
