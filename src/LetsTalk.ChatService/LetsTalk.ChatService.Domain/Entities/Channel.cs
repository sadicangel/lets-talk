using LetsTalk.Shared;

namespace LetsTalk.ChatService.Domain.Entities;

public sealed class Channel
{
    public required string Id { get; init; }
    public uint Version { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<ChannelMember> Members { get; init; } = default!;
    public List<ChannelMessage> Messages { get; init; } = default!;

    public ChannelIdentity ToIdentity() => new(Id, Name, Description);
}
