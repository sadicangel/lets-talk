namespace LetsTalk.Identity.Services;

public sealed class JwtBearerTokenProviderOptions
{
    public required string SecurityKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required long TokenExpirationSeconds { get; init; }
    public required int RefreshTokenSize { get; init; }
}
