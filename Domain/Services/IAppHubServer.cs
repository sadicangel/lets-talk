namespace LetsTalk.Services;

public interface IAppHubServer
{
    Task JoinChannel(string channelId);
    Task LeaveChannel(string channelId);

    Task SendMessage(string channelId, ContentType contentType, byte[] content);
}