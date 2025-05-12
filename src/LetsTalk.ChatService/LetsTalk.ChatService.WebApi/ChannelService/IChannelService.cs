using Refit;

namespace LetsTalk.ChatService.WebApi.ChannelService;

public interface IChannelService
{
    [Get("/api/v1/channels")]
    Task<UserChannelListResponse> GetUserChannelListAsync(string userId, CancellationToken cancellationToken = default);
}
