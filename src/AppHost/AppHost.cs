using System.Security.Cryptography;
using LetsTalk;

var builder = DistributedApplication.CreateBuilder(args);

var securityKey = builder.AddParameter("jwt-security-key", Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), secret: true);

var postgres = builder.AddPostgres();

var identityService = builder.AddIdentityService(postgres, securityKey);

var chatService = builder.AddChatService(postgres, securityKey)
    .WithReference(identityService);

var cliApp = builder.AddCliApp(identityService, chatService);

builder.Build().Run();
