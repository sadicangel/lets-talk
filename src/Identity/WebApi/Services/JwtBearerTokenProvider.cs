using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LetsTalk.Identity.Services;

public sealed class JwtBearerTokenProvider(IOptions<JwtBearerTokenProviderOptions> options, TimeProvider timeProvider)
{
    private readonly SigningCredentials _signingCredentials = new(
        key: new SymmetricSecurityKey(Convert.FromBase64String(options.Value.SecurityKey)),
        algorithm: SecurityAlgorithms.HmacSha256);

    public Task<(string AccessToken, string RefreshToken, long ExpiresIn)> GenerateAsync(AppUser user)
    {
        var utcNow = timeProvider.GetUtcNow().DateTime;

        var securityToken = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Picture, user.AvatarUrl ?? ""),
            ],
            expires: utcNow.AddSeconds(options.Value.TokenExpirationSeconds),
            signingCredentials: _signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(options.Value.RefreshTokenSize));

        return Task.FromResult((accessToken, refreshToken, options.Value.TokenExpirationSeconds));
    }
}
