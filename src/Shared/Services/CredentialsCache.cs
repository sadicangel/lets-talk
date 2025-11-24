using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.Options;

namespace LetsTalk.Services;

public sealed class CredentialsCache(IOptions<CredentialsCacheOptions> options, IIdentityApi identityService)
{
    private AccessTokenResponse? _response;
    private DateTimeOffset _expires;

    public async Task<string> GetBearerTokenAsync(CancellationToken cancellationToken)
    {
        if (_response is null)
        {
            _response = await identityService.LoginAsync(new LoginRequest(options.Value.Username, options.Value.Password), cancellationToken);
            _expires = DateTimeOffset.UtcNow.AddSeconds(_response.ExpiresIn - 30);
        }
        else if (_expires <= DateTimeOffset.UtcNow)
        {
            _response = await identityService.RefreshAsync(new RefreshRequest(_response.RefreshToken), cancellationToken);
            _expires = DateTimeOffset.UtcNow.AddSeconds(_response.ExpiresIn - 30);
        }

        return _response.AccessToken;
    }
}
