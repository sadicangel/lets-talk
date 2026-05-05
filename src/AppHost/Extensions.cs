using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using LetsTalk.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LetsTalk;

internal static class Extensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<PostgresServerResource> AddPostgres()
        {
            var username = builder.AddParameter("postgres-username", "postgres");
            var password = builder.AddParameter("postgres-password", PasswordGenerator.Generate(12), secret: true);

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
                .WithEnvironment(LetsTalkJwtOptions.GetEnvironmentVariableName(x => x.SecurityKey), securityKey)
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
                .WithEnvironment(LetsTalkJwtOptions.GetEnvironmentVariableName(x => x.SecurityKey), securityKey)
                .WithReference(database)
                .WaitFor(dbMigration);
        }

        public IResourceBuilder<ProjectResource> AddCliApp(IResourceBuilder<ProjectResource> identityService, IResourceBuilder<ProjectResource> chatService)
        {
            var username = builder.AddParameter("cli-username", "ShadySardine");
            var password = builder.AddParameter("cli-password", PasswordGenerator.Generate(12), secret: true);

            return builder.AddProject<Projects.CliApp>("cli-app")
                .WithExplicitStart()
                .ExcludeFromManifest()
                .WithEnvironment(CredentialsCacheOptions.GetEnvironmentVariableName(x => x.Username), username)
                .WithEnvironment(CredentialsCacheOptions.GetEnvironmentVariableName(x => x.Password), password)
                .WaitFor(identityService)
                .WithEnvironment(IdentityApiOptions.GetEnvironmentVariableName(x => x.Url), identityService.GetEndpoint("https"))
                .WaitFor(chatService)
                .WithEnvironment(ChatApiOptions.GetEnvironmentVariableName(x => x.Url), chatService.GetEndpoint("https"));
        }
    }

    extension<T>(IResourceBuilder<T> builder) where T : IResourceWithEndpoints
    {
        private IResourceBuilder<T> WithOpenApiReference()
        {
            return builder.WithCommand(
                "scalar-api-reference",
                "Scalar API Reference",
                _ =>
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

file static class LetsTalkOptionsExtensions
{
    extension<TOptions>(TOptions) where TOptions : ILetsTalkOptions
    {
        public static string GetEnvironmentVariableName<TProperty>(Expression<Func<TOptions, TProperty>> expression) =>
            GetEnvironmentVariableName(TOptions.SectionName, expression);
    }

    extension<T>(T)
    {
        private static string GetEnvironmentVariableName<TProperty>(string sectionName, Expression<Func<T, TProperty>> expression) =>
            GetFullPath(sectionName, expression);
    }

    private static string GetFullPath<T, TProperty>(string sectionName, Expression<Func<T, TProperty>> expression)
    {
        var builder = new StringBuilder(sectionName).Replace(":", "__");
        var current = expression.Body;

        while (current is MemberExpression member)
        {
            builder.Append($"__{member.Member.Name}");
            current = member.Expression;
        }

        return builder.ToString();
    }
}
