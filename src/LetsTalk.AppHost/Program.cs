using System.Security.Cryptography;

var builder = DistributedApplication.CreateBuilder(args);

var securityKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

var postgres = builder.AddPostgres("letstalk-postgres")
    .WithDataVolume()
    .WithPgWeb();

var identityDatabase = postgres.AddDatabase("letstalk-identity-service");
var identityDbMigration = builder.AddProject<Projects.LetsTalk_IdentityService_DbMigration>("letstalk-identity-service-db-migration")
    .WithReference(identityDatabase);

var identityService = builder.AddProject<Projects.LetsTalk_IdentityService_WebApi>("letstalk-identity-service-webapi")
    .WithOpenApiReference()
    .WithEnvironment("Jwt__SecurityKey", securityKey)
    .WithReference(identityDatabase)
    .WaitFor(identityDbMigration);

var chatDatabase = postgres.AddDatabase("letstalk-chat-service");
var chatService = builder.AddProject<Projects.LetsTalk_ChatService_WebApi>("letstalk-chat-service-webapi")
    .WithOpenApiReference()
    .WithEnvironment("Jwt__SecurityKey", securityKey)
    .WithReference(chatDatabase)
    .WithReference(identityService);

builder.AddProject<Projects.LetsTalk_ChatClient_Console>("letstalk-chat-client-console")
    .WithOpenApiReference()
    .WithReference(identityService)
    .WithReference(chatService)
    .WaitFor(identityService)
    .WaitFor(chatService);

builder.Build().Run();
