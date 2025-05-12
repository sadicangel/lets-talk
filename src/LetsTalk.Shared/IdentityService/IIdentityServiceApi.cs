using Microsoft.AspNetCore.Authentication.BearerToken;
using Refit;

namespace LetsTalk.Shared.IdentityService;

public interface IIdentityServiceApi
{
    [Post("/api/register")]
    Task RegisterAsync([Body] RegisterRequest request, CancellationToken cancellationToken = default);

    [Post("/api/login")]
    Task<AccessTokenResponse> LoginAsync([Body] LoginRequest request, CancellationToken cancellationToken = default);

    [Get("/api/refresh")]
    Task<AccessTokenResponse> RefreshAsync([Body] RefreshRequest refreshRequest, CancellationToken cancellationToken = default);
}

public sealed record class RegisterRequest(string UserName, string Password, string Email);
public sealed record class LoginRequest(string UserName, string Password);
public sealed record class RefreshRequest(string RefreshToken);
