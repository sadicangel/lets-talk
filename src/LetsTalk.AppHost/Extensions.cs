using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting;

public static class Extensions
{
    public static IResourceBuilder<T> WithOpenApiReference<T>(this IResourceBuilder<T> builder, string name, string displayName, string path)
        where T : IResourceWithEndpoints
    {
        return builder.WithCommand(
            name,
            displayName,
            executeCommand: context =>
            {
                try
                {
                    var url = $"{builder.GetEndpoint("https").Url}/{path}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    return Task.FromResult(new ExecuteCommandResult { Success = true });
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new ExecuteCommandResult { Success = false, ErrorMessage = ex.ToString() });
                }
            },
            updateState: context => context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy ? ResourceCommandState.Enabled : ResourceCommandState.Disabled,
            iconName: "DocumentChevronDouble",
            iconVariant: IconVariant.Regular);
    }
}
