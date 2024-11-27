var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("letstalk-postgres").WithPgAdmin();
var letsTalkDatabase = postgres.AddDatabase("letstalk-database");

var webapi = builder.AddProject<Projects.LetsTalk_WebApi>("letstalk-webapi")
    .WithReference(letsTalkDatabase)
    .WaitFor(letsTalkDatabase);

builder.AddNpmApp("letstalk-webapp", "../LetsTalk.WebApp")
    .WithReference(webapi)
    .WaitFor(webapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
