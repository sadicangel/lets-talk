namespace System.Security.Claims;

public readonly record struct UserInfo(string UserId, string UserName, string? AvatarUrl);

public static class UserClaimsExtensions
{
    public static UserInfo GetUserInfo(this ClaimsPrincipal? claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);

        var userId = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier);
        var userName = claimsPrincipal?.FindFirst(ClaimTypes.Name);
        var avatarUrl = claimsPrincipal?.FindFirst(ClaimTypes.UserData);

        return new UserInfo(
            userId?.Value ?? string.Empty,
            userName?.Value ?? string.Empty,
            avatarUrl?.Value);
    }
}
