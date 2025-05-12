using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LetsTalk.IdentityService.WebApi.Services;

public sealed class JwtBearerTokenProvider(IOptions<JwtBearerTokenProviderOptions> options, TimeProvider timeProvider)
{
    private readonly SigningCredentials _signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Convert.FromBase64String(options.Value.SecurityKey)),
        SecurityAlgorithms.HmacSha256);

    public Task<(string AccessToken, string RefreshToken, long ExpiresIn)> GenerateAsync(IdentityUser user)
    {
        var utcNow = timeProvider.GetUtcNow().DateTime;

        var securityToken = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: [
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Picture, ""),
            ],
            expires: utcNow.AddSeconds(options.Value.TokenExpirationSeconds),
            signingCredentials: _signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(options.Value.RefreshTokenSize));

        return Task.FromResult((accessToken, refreshToken, options.Value.TokenExpirationSeconds));
    }
}
