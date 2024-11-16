namespace LetsTalk.WebApi.Entities;

public sealed class User : IEntity
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset? LastSeenAt { get; set; }

    public static User Create(Guid userId, string userName, string email, string passwordHash, string passwordSalt, DateTimeOffset createdAt, DateTimeOffset? lastSeenAt = null) => new()
    {
        Id = userId,
        UserName = userName,
        Email = email,
        PasswordHash = passwordHash,
        PasswordSalt = passwordSalt,
        CreatedAt = createdAt,
        LastSeenAt = lastSeenAt,
    };
}
