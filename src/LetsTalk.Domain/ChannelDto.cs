namespace LetsTalk.Domain;
public sealed record class ChannelDto(
    string Id,
    string DisplayName,
    string? IconUrl,
    string AdminId) : IEquatable<ChannelDto>
{
    public bool Equals(ChannelDto? other) => other is not null && Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(Id);
}
