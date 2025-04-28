using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting;

public static class Extensions
{
    public static IResourceBuilder<T> WithOpenApiReference<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
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
                    return Task.FromResult(new ExecuteCommandResult { Success = false, ErrorMessage = ex.ToString() });
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
