using System.Security.Claims;
using LetsTalk.WebApi.Entities;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.WebApi;
public static class CustomClaimTypes
{
    public const string ImageUrl = "http://schemas.letstalk.com/claims/imageurl";
}

internal static class ClaimExtensions
{
    public static Guid GetUserId(this HubCallerContext context) =>
        Guid.Parse(context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    public static string GetUserName(this HubCallerContext context) =>
        context.User?.FindFirst(ClaimTypes.Name)?.Value!;

    public static string? GetUserAvatarUri(this HubCallerContext context) =>
        context.User?.FindFirst(CustomClaimTypes.ImageUrl)?.Value;

    public static ClaimsIdentity GetIdentity(this User user, string authenticationScheme)
    {
        var identity = new ClaimsIdentity(
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
            ],
            authenticationScheme);

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            identity.AddClaim(new(CustomClaimTypes.ImageUrl, user.AvatarUrl));
        }

        return identity;
    }
}
