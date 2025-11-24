namespace LetsTalk.Services;

public sealed class CredentialsCacheOptions
{
    public const string SectionName = "LetsTalk:Credentials";

    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}
