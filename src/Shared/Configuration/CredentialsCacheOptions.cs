namespace LetsTalk.Configuration;

public sealed class CredentialsCacheOptions : ILetsTalkOptions
{
    public const string SectionName = "LetsTalk:Credentials";
    static string ILetsTalkOptions.SectionName => SectionName;

    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}
