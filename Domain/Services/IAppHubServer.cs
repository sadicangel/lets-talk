namespace LetsTalk.Services;

public interface IAppHubServer
{
    Task CreateChannel(string channelName, string? channelIcon);
    Task DeleteChannel(string channelId);

    Task JoinChannel(string channelId);
    Task LeaveChannel(string channelId);

    Task SendMessage(string channelId, string contentType, byte[] content);
}