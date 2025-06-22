using Refit;

namespace LetsTalk.Shared.Services;
public interface IChatService
{
    [Post("/api/system/notification")]
    [Headers("Authorization: Bearer")]
    public Task SendNotification([Body] NotificationRequest request, CancellationToken cancellationToken = default);

    [Get("/api/channels")]
    [Headers("Authorization: Bearer")]
    public Task<ChannelListResponse> GetChannels(string? userId = null, CancellationToken cancellationToken = default);

    [Get("/api/channels/{channelId}")]
    [Headers("Authorization: Bearer")]
    public Task<ChannelResponse> GetChannel(string channelId, CancellationToken cancellationToken = default);

    [Post("/api/channels")]
    [Headers("Authorization: Bearer")]
    public Task<ChannelResponse> CreateChannel([Body] ChannelRequest request, CancellationToken cancellationToken = default);

    [Put("/api/channels/{channelId}")]
    [Headers("Authorization: Bearer")]
    public Task<ChannelResponse> UpdateChannel(string channelId, [Body] ChannelRequest request, CancellationToken cancellationToken = default);

    [Delete("/api/channels/{channelId}")]
    [Headers("Authorization: Bearer")]
    public Task DeleteChannel(string channelId, CancellationToken cancellationToken = default);
}

public sealed record class NotificationRequest(string ContentType, byte[] Content);

public sealed record class ChannelListResponse(string[] Channels);
public sealed record class ChannelResponse(string ChannelId, string ChannelName, string? Description, List<string> Members);
public sealed record class ChannelRequest(string Name, string? Description);
