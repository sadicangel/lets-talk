using System.Security.Claims;
using LetsTalk.Models;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LetsTalk;

public static class UserClaimsExtensions
{
    extension(ClaimsPrincipal? claimsPrincipal)
    {
        public string UserId
        {
            get
            {
                ArgumentNullException.ThrowIfNull(claimsPrincipal);
                var userId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub);
                return userId?.Value ?? string.Empty;
            }
        }

        public string UserName
        {
            get
            {
                ArgumentNullException.ThrowIfNull(claimsPrincipal);
                var userName = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Name);
                return userName?.Value ?? string.Empty;
            }
        }

        public string Email
        {
            get
            {
                ArgumentNullException.ThrowIfNull(claimsPrincipal);
                var email = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Email);
                return email?.Value ?? string.Empty;
            }
        }

        public string? AvatarUrl
        {
            get
            {
                ArgumentNullException.ThrowIfNull(claimsPrincipal);
                var avatarUrl = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Picture);
                return avatarUrl?.Value;
            }
        }

        public UserIdentity UserIdentity
        {
            get
            {
                ArgumentNullException.ThrowIfNull(claimsPrincipal);

                return new UserIdentity(
                    claimsPrincipal.UserId,
                    claimsPrincipal.UserName,
                    claimsPrincipal.Email,
                    claimsPrincipal.AvatarUrl);
            }
        }
    }
}
