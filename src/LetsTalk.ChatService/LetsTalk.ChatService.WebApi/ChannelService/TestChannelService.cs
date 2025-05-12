namespace LetsTalk.ChatService.WebApi.ChannelService;

public sealed class TestChannelService : IChannelService
{
    public Task<UserChannelListResponse> GetUserChannelListAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UserChannelListResponse(
        [
            new UserChannelResponse(
                Id: "1",
                Name: "Test Channel",
                Description: "This is a test channel.",
                Members:
                [
                    new UserChannelMemberResponse(
                        Id: "1",
                        UserId: userId,
                        IsOwner: false,
                        IsAdmin: false,
                        IsBlocked: false,
                        IsBanned: false)
                ])
        ]));
    }
}
