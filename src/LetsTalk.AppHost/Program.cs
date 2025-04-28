var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("letstalk-postgres")
    .WithDataVolume()
    .WithPgWeb();

var chatDatabase = postgres.AddDatabase("letstalk-chat-service");

builder.AddProject<Projects.LetsTalk_ChatService_WebApi>("letstalk-chat-service-webapi")
    .WithOpenApiReference()
    .WithReference(chatDatabase);

builder.Build().Run();
