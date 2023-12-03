#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
using Microsoft.AspNetCore.Identity;

namespace LetsTalk;

public sealed class User : IdentityUser
{
    [ProtectedPersonalData]
    public override string UserName { get; set; } = null!;

    public override string? NormalizedUserName { get; set; } = null!;

    [ProtectedPersonalData]
    public override string? Email { get; set; } = null!;

    public string? Avatar { get; set; }

    public List<string> Channels { get; init; } = [];
}

public sealed class UserClaim : IdentityUserClaim<string>;

public sealed class UserLogin : IdentityUserLogin<string>;

public sealed class UserToken : IdentityUserToken<string>;

#pragma warning restore CS8765
