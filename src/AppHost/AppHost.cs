using System.Security.Cryptography;
using LetsTalk;

var builder = DistributedApplication.CreateBuilder(args);

var securityKey = builder.AddParameter("jwt-security-key", Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), secret: true);

var postgres = builder.AddPostgres();

var identityService = builder.AddIdentityService(postgres, securityKey);

var chatService = builder.AddChatService(postgres, securityKey)
    .WithReference(identityService);

var npm = OperatingSystem.IsWindows() ? "npm.cmd" : "npm";
var aspNetCoreDevCertPath = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "..", "..", ".dev-certs", "aspnetcore-dev-cert.pem"));

builder.AddExecutable("web", npm, "../Apps/Web", "run", "dev")
    .WithEnvironment("NODE_OPTIONS", "--use-system-ca")
    .WithEnvironment("NODE_EXTRA_CA_CERTS", aspNetCoreDevCertPath)
    .WithEnvironment("IDENTITY_API_URL", identityService.GetEndpoint("https"))
    .WithEnvironment("CHAT_API_URL", chatService.GetEndpoint("https"))
    .WithEnvironment("PUBLIC_CHAT_API_URL", chatService.GetEndpoint("https"))
    .WithHttpEndpoint(port: 5173, targetPort: 5173, name: "http", isProxied: false)
    .WaitFor(identityService)
    .WaitFor(chatService);

var cliApp = builder.AddCliApp(identityService, chatService);

builder.Build().Run();
