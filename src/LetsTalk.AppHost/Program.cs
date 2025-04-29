var builder = DistributedApplication.CreateBuilder(args);


var postgres = builder.AddPostgres("letstalk-postgres")
    .WithDataVolume()
    .WithPgWeb();


var chatDatabase = postgres.AddDatabase("letstalk-chat-service");
var chatService = builder.AddProject<Projects.LetsTalk_ChatService_WebApi>("letstalk-chat-service-webapi")
    .WithOpenApiReference()
    .WithReference(chatDatabase);

builder.AddProject<Projects.LetsTalk_ChatClient_Console>("chat-client")
    .WithReference(chatService);

builder.Build().Run();
