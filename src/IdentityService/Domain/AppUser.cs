using Microsoft.AspNetCore.Identity;

namespace LetsTalk.Identity;

public sealed class AppUser : IdentityUser
{
    public string? AvatarUrl { get; set; }
}
