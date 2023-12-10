using LetsTalk.Responses;

namespace LetsTalk.Channels;

public sealed record class ChannelListResponse(IReadOnlyList<ChannelProfile> Channels);
