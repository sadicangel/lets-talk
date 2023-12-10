namespace LetsTalk;

public sealed record UserChannel
{
    public required string UserId { get; init; }
    public required User User { get; init; }
    public required string ChannelId { get; init; }
    public required Channel Channel { get; init; }
    public required DateTimeOffset MemberSince { get; init; }
}
