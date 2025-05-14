namespace LetsTalk.ChannelService.WebApi.Entities;

public sealed class Channel
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<ChannelMember> Members { get; init; } = default!;
}
