using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LetsTalk.Shared;

public readonly record struct UserIdentity(string UserId, string UserName, string? AvatarUrl);

public static class UserClaimsExtensions
{
    public static string GetUserId(this ClaimsPrincipal? claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);
        var userId = claimsPrincipal?.FindFirst(JwtRegisteredClaimNames.Sub);
        return userId?.Value ?? string.Empty;
    }

    public static string GetUserName(this ClaimsPrincipal? claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);
        var userName = claimsPrincipal?.FindFirst(JwtRegisteredClaimNames.Name);
        return userName?.Value ?? string.Empty;
    }

    public static string? GetAvatarUrl(this ClaimsPrincipal? claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);
        var avatarUrl = claimsPrincipal?.FindFirst(JwtRegisteredClaimNames.Picture);
        return avatarUrl?.Value;
    }

    public static UserIdentity GetUserIdentity(this ClaimsPrincipal? claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);

        return new UserIdentity(
            claimsPrincipal.GetUserId(),
            claimsPrincipal.GetUserName(),
            claimsPrincipal.GetAvatarUrl());
    }
}
