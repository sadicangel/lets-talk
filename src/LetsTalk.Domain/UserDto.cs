namespace LetsTalk.Domain;

public sealed record class UserDto(
    string Id,
    string UserName,
    string? AvatarUrl) : IEquatable<UserDto>
{
    public bool IsOnline { get; set; }

    public bool Equals(UserDto? other) => other is not null && Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(Id);
}
