namespace LetsTalk.Services;

public interface IChatHubServer
{
    Task SendMessage(string channelId, string contentType, byte[] content, CancellationToken cancellationToken = default);
}
