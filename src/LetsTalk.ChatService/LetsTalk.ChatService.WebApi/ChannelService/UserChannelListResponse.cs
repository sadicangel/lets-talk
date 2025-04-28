namespace LetsTalk.ChatService.WebApi.ChannelService;

public record UserChannelListResponse(UserChannelResponse[] Channels);

public record UserChannelResponse(
    string Id,
    string Name,
    string Description,
    UserChannelMemberResponse[] Members);

public record UserChannelMemberResponse(
    string Id,
    string UserId,
    bool IsOwner,
    bool IsAdmin,
    bool IsBlocked,
    bool IsBanned);
