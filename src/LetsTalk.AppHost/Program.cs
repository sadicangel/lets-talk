var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("letstalk-postgres").WithPgAdmin();
var letsTalkDatabase = postgres.AddDatabase("letstalk-database");

builder.AddProject<Projects.LetsTalk_WebApi>("letstalk-webapi")
    .WithReference(letsTalkDatabase)
    .WaitFor(letsTalkDatabase);

builder.Build().Run();
