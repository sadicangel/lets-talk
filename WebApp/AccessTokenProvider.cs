using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity.Data;

namespace LetsTalk;

public sealed class AccessTokenProvider(IConfiguration configuration, HttpClient httpClient)
{
    private readonly string _loginUri = $"{configuration.GetRequiredSection("Services:webapi:1").Value!}/account/login";
    private readonly string _refreshUri = $"{configuration.GetRequiredSection("Services:webapi:1").Value!}/account/refresh";

    private AccessTokenResponse? _accessTokenResponse;
    private DateTimeOffset _expiresAt;

    public async Task<AccessTokenResponse> ProvideAccessTokenAsync()
    {
        if (_accessTokenResponse is null)
        {
            var response = await httpClient.PostAsJsonAsync(_loginUri, new LoginRequest
            {
                Email = "user@lt.com",
                Password = "Pass@123"
            });

            response.EnsureSuccessStatusCode();

            _accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(_accessTokenResponse!.ExpiresIn);
        }
        else if (DateTimeOffset.UtcNow >= _expiresAt)
        {
            var response = await httpClient.PostAsJsonAsync(_refreshUri, new RefreshRequest { RefreshToken = _accessTokenResponse.RefreshToken });

            response.EnsureSuccessStatusCode();

            _accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(_accessTokenResponse!.ExpiresIn);
        }

        return _accessTokenResponse;
    }
}