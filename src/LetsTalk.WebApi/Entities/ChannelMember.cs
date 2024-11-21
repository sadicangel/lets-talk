namespace LetsTalk.WebApi.Entities;

public sealed class ChannelMember
{
    public required string ChannelId { get; set; }
    public Channel Channel { get; set; } = default!;
    public required string UserId { get; set; }
    public User User { get; set; } = default!;
    public required DateTimeOffset JoinedAt { get; set; }
}
