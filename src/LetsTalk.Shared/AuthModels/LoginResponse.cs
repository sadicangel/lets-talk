namespace LetsTalk.Shared.AuthModels;

public sealed record class LoginResponse(UserIdentity User, string AccessToken, DateTime Expiration);
