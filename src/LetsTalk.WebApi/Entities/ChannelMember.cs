namespace LetsTalk.WebApi.Entities;

public sealed class ChannelMember
{
    public required Guid ChannelId { get; set; }
    public required Guid UserId { get; set; }
    public required DateTimeOffset JoinedAt { get; set; }
}
