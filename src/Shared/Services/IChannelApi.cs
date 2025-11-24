using Refit;

namespace LetsTalk.Services;

public interface IChannelApi
{
    [Get("/api/channels")]
    [Headers("Authorization: Bearer")]
    Task<ChannelListResponse> GetChannels(string? userId = null, CancellationToken cancellationToken = default);

    [Get("/api/channels/{channelId}")]
    [Headers("Authorization: Bearer")]
    Task<ChannelResponse> GetChannel(string channelId, CancellationToken cancellationToken = default);

    [Post("/api/channels")]
    [Headers("Authorization: Bearer")]
    Task<ChannelResponse> CreateChannel([Body] ChannelRequest request, CancellationToken cancellationToken = default);

    [Put("/api/channels/{channelId}")]
    [Headers("Authorization: Bearer")]
    Task<ChannelResponse> UpdateChannel(string channelId, [Body] ChannelRequest request, CancellationToken cancellationToken = default);

    [Delete("/api/channels/{channelId}")]
    [Headers("Authorization: Bearer")]
    Task DeleteChannel(string channelId, CancellationToken cancellationToken = default);

    [Patch("/api/channels/{channelId}/join")]
    [Headers("Authorization: Bearer")]
    Task JoinChannel(string channelId, CancellationToken cancellationToken = default);

    [Patch("/api/channels/{channelId}/leave")]
    [Headers("Authorization: Bearer")]
    Task LeaveChannel(string channelId, CancellationToken cancellationToken = default);
}

public sealed record class ChannelListResponse(ChannelResponse[] Channels);

public sealed record class ChannelResponse(string ChannelId, string ChannelName, string? Description, List<string> Members);

public sealed record class ChannelRequest(string Name, string? Description);
