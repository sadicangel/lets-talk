using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LetsTalk;

internal static class Extensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<PostgresServerResource> AddPostgres()
        {
            var username = builder.AddParameter("postgres-username", "postgres");
            var password = builder.AddParameter("postgres-password", Convert.ToBase64String(RandomNumberGenerator.GetBytes(12)), secret: true);

            return builder.AddPostgres("postgres", username, password).WithPgAdmin();
        }

        public IResourceBuilder<ProjectResource> AddIdentityService(
            IResourceBuilder<PostgresServerResource> postgres,
            IResourceBuilder<ParameterResource> securityKey)
        {
            var database = postgres.AddDatabase("identity-db");
            var dbMigration = builder.AddProject<Projects.IdentityDbMigration>("identity-db-migration")
                .WithReference(database)
                .WaitFor(database);

            return builder.AddProject<Projects.IdentityWebApi>("identity-webapi")
                .WithOpenApiReference()
                .WithEnvironment("LetsTalk__Jwt__SecurityKey", securityKey)
                .WithReference(database)
                .WaitFor(dbMigration);
        }

        public IResourceBuilder<ProjectResource> AddChatService(
            IResourceBuilder<PostgresServerResource> postgres,
            IResourceBuilder<ParameterResource> securityKey)
        {
            var database = postgres.AddDatabase("chat-db");
            var dbMigration = builder.AddProject<Projects.ChatDbMigration>("chat-db-migration")
                .WithReference(database)
                .WaitFor(database);

            return builder.AddProject<Projects.ChatWebApi>("chat-webapi")
                .WithOpenApiReference()
                .WithEnvironment("LetsTalk__Jwt__SecurityKey", securityKey)
                .WithReference(database)
                .WaitFor(dbMigration);
        }

        public IResourceBuilder<ProjectResource> AddCliApp(IResourceBuilder<ProjectResource> identityService, IResourceBuilder<ProjectResource> chatService)
        {
            var username = builder.AddParameter("cli-username", "ShadySardine");
            var password = builder.AddParameter("cli-password", Convert.ToBase64String(RandomNumberGenerator.GetBytes(12)), secret: true);

            return builder.AddProject<Projects.CliApp>("cli-app")
                .WithExplicitStart()
                .ExcludeFromManifest()
                .WithEnvironment("LetsTalk__Credentials__UserName", username)
                .WithEnvironment("LetsTalk__Credentials__Password", password)
                .WithReference(identityService)
                .WithEnvironment("LetsTalk__IdentityApi__Url", $"https+http://{identityService.Resource.Name}")
                .WithReference(chatService)
                .WithEnvironment("LetsTalk__ChatApi__Url", $"https+http://{chatService.Resource.Name}")
                .WithReference(chatService)
                .WithEnvironment("LetsTalk__ChatHub__Url", $"https+http://{chatService.Resource.Name}/hub");
        }
    }

    extension<T>(IResourceBuilder<T> builder) where T : IResourceWithEndpoints
    {
        private IResourceBuilder<T> WithOpenApiReference()
        {
            return builder.WithCommand(
                "scalar-api-reference",
                "Scalar API Reference",
                context =>
                {
                    try
                    {
                        var url = $"{builder.GetEndpoint("https").Url}/scalar/v1";
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                        return Task.FromResult(new ExecuteCommandResult { Success = true });
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult(
                            new ExecuteCommandResult
                            {
                                Success = false,
                                ErrorMessage = ex.ToString()
                            });
                    }
                },
                new CommandOptions
                {
                    Description = "Open the OpenAPI reference in a browser.",
                    IconName = "DocumentChevronDouble",
                    IconVariant = IconVariant.Regular,
                    UpdateState = context => context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy ? ResourceCommandState.Enabled : ResourceCommandState.Disabled,
                });
        }
    }
}
