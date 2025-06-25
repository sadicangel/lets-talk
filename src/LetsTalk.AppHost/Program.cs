using System.Security.Cryptography;

var builder = DistributedApplication.CreateBuilder(args);

var securityKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

var rabbitmqUsername = builder.AddParameter("username", "guest");
var rabbitmqPassword = builder.AddParameter("password", "guest");

var rabbitmq = builder.AddRabbitMQ("letstalk-rabbitmq", rabbitmqUsername, rabbitmqPassword)
    .WithManagementPlugin();

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

var chatDatabase = postgres.AddDatabase("letstalk-chat-service-db");
var chatDbMigration = builder.AddProject<Projects.LetsTalk_ChatService_DbMigration>("letstalk-chat-service-db-migration")
    .WithReference(chatDatabase)
    .WaitFor(chatDatabase);
var chatService = builder.AddProject<Projects.LetsTalk_ChatService_WebApi>("letstalk-chat-service-webapi")
    .WithOpenApiReference()
    .WithEnvironment("Jwt__SecurityKey", securityKey)
    .WithReference(rabbitmq)
    .WithReference(chatDatabase)
    .WithReference(identityService)
    .WaitFor(rabbitmq)
    .WaitFor(chatDatabase)
    .WaitFor(chatDbMigration)
    .WaitFor(identityService);
builder.AddProject<Projects.LetsTalk_ChatService_MessageConsumer>("letstalk-chat-service-message-consumer")
    .WithReference(rabbitmq)
    .WithReference(chatDatabase)
    .WaitFor(rabbitmq)
    .WaitFor(chatDatabase)
    .WaitFor(chatDbMigration);

builder.AddProject<Projects.LetsTalk_ChatClient_Console>("letstalk-chat-client-console")
    .WithReference(identityService)
    .WithReference(chatService)
    .WaitFor(identityService)
    .WaitFor(chatService)
    .WithExplicitStart()
    .ExcludeFromManifest();

builder.Build().Run();
