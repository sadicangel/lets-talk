using System.Security.Cryptography;

var builder = DistributedApplication.CreateBuilder(args);

var securityKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

var postgres = builder.AddPostgres("letstalk-postgres")
    .WithDataVolume()
    .WithPgWeb();

var identityDatabase = postgres.AddDatabase("letstalk-identity-service-db");
var identityDbMigration = builder.AddProject<Projects.LetsTalk_IdentityService_DbMigration>("letstalk-identity-service-db-migration")
    .WithReference(identityDatabase)
    .WaitFor(identityDatabase);
var identityService = builder.AddProject<Projects.LetsTalk_IdentityService_WebApi>("letstalk-identity-service-webapi")
    .WithOpenApiReference()
    .WithEnvironment("Jwt__SecurityKey", securityKey)
    .WithReference(identityDatabase)
    .WaitFor(identityDbMigration);

var channelDatabase = postgres.AddDatabase("letstalk-channel-service-db");
var channelDbMigration = builder.AddProject<Projects.LetsTalk_ChannelService_DbMigration>("letstalk-channel-service-db-migration")
    .WithReference(channelDatabase)
    .WaitFor(channelDatabase);
var channelService = builder.AddProject<Projects.LetsTalk_ChannelService_WebApi>("letstalk-channel-service-webapi")
    .WithOpenApiReference()
    .WithEnvironment("Jwt__SecurityKey", securityKey)
    .WithReference(channelDatabase)
    .WithReference(identityService)
    .WaitFor(channelDbMigration);

var chatDatabase = postgres.AddDatabase("letstalk-chat-service-db");
var chatService = builder.AddProject<Projects.LetsTalk_ChatService_WebApi>("letstalk-chat-service-webapi")
    .WithOpenApiReference()
    .WithEnvironment("Jwt__SecurityKey", securityKey)
    .WithReference(chatDatabase)
    .WithReference(channelService)
    .WithReference(identityService);

builder.AddProject<Projects.LetsTalk_ChatClient_Console>("letstalk-chat-client-console")
    .WithReference(identityService)
    .WithReference(channelService)
    .WithReference(chatService)
    .WaitFor(identityService)
    .WaitFor(channelService)
    .WaitFor(chatService)
    .WithExplicitStart()
    .ExcludeFromManifest();

builder.Build().Run();
