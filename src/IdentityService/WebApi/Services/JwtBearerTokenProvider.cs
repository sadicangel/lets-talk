using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using LetsTalk.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LetsTalk.Identity.Services;

public sealed class JwtBearerTokenProvider(IOptions<LetsTalkJwtOptions> jwtOptions, TimeProvider timeProvider)
{
    private static readonly JwtSecurityTokenHandler s_tokenHandler = new JwtSecurityTokenHandler();
    private readonly SigningCredentials _signingCredentials = new SigningCredentials(key: new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(jwtOptions.Value.SecurityKey)), algorithm: SecurityAlgorithms.HmacSha256);

    public ValueTask<(string AccessToken, string RefreshToken, long ExpiresIn)> GenerateAsync(AppUser user)
    {
        var utcNow = timeProvider.GetUtcNow().DateTime;
        var jwt = jwtOptions.Value;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            Subject = user.ToSubject(),
            Expires = utcNow.AddSeconds(jwt.TokenExpirationSeconds),
            SigningCredentials = _signingCredentials
        };

        var accessToken = s_tokenHandler.CreateEncodedJwt(descriptor);
        var refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(jwt.RefreshTokenSize));

        return ValueTask.FromResult((accessToken, refreshToken, jwt.TokenExpirationSeconds));
    }
}

file static class AppUserExtensions
{
    extension(AppUser user)
    {
        public ClaimsIdentity ToSubject() => new ClaimsIdentity(
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Picture, user.AvatarUrl ?? ""),
        ]);
    }
}
