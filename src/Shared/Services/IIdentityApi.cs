using Microsoft.AspNetCore.Authentication.BearerToken;
using Refit;

namespace LetsTalk.Services;

public interface IIdentityApi
{
    [Post("/api/register")]
    Task RegisterAsync([Body] RegisterRequest request, CancellationToken cancellationToken = default);

    [Post("/api/login")]
    Task<AccessTokenResponse> LoginAsync([Body] LoginRequest request, CancellationToken cancellationToken = default);

    [Get("/api/refresh")]
    Task<AccessTokenResponse> RefreshAsync([Body] RefreshRequest refreshRequest, CancellationToken cancellationToken = default);

    [Get("/api/profile")]
    [Headers("Authorization: Bearer")]
    Task<UserProfileResponse> GetProfileAsync(CancellationToken cancellationToken = default);

    [Put("/api/profile")]
    [Headers("Authorization: Bearer")]
    Task<UserProfileResponse> UpdateProfileAsync(UserProfileRequest request, CancellationToken cancellationToken = default);
}

public sealed record class RegisterRequest(string UserName, string Password, string Email, string? AvatarUrl);

public sealed record class LoginRequest(string UserName, string Password);

public sealed record class RefreshRequest(string RefreshToken);

public sealed record class UserProfileRequest(string Email, string? AvatarUrl);

public sealed record class UserProfileResponse(string UserId, string UserName, string Email, string? AvatarUrl);
