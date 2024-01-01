namespace LetsTalk.Account.Dtos;

public sealed record RegisterRequest(string Email, string UserName, string Password);
