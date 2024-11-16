using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.WebApi;

internal static class HubContextExtensions
{
    public static Guid GetUserId(this HubCallerContext context) =>
        Guid.Parse(context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
}
