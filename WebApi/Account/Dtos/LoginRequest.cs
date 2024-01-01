namespace LetsTalk.Account.Dtos;

public sealed record class LoginRequest(string Email, string Password, bool RememberMe);
