using LetsTalk.Shared.Events;
using Microsoft.AspNetCore.SignalR.Client;

namespace LetsTalk.Shared.Services;

public abstract class LetsTalkClient : ILetsTalkClient, IAsyncDisposable
{
    private readonly HubConnection _connection;

    protected LetsTalkClient(HubConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _connection.On<ChannelMessage>(nameof(OnMessage), OnMessage);
        _connection.On<ServerNotification>(nameof(OnNotification), OnNotification);
        _connection.On<UserConnected>(nameof(OnUserConnected), OnUserConnected);
        _connection.On<UserDisconnected>(nameof(OnUserDisconnected), OnUserDisconnected);
        _connection.On<ChannelMemberJoined>(nameof(OnChannelMemberJoined), OnChannelMemberJoined);
        _connection.On<ChannelMemberLeft>(nameof(OnChannelMemberLeft), OnChannelMemberLeft);
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default) => _connection.StartAsync(cancellationToken);

    public Task DisconnectAsync(CancellationToken cancellationToken = default) => _connection.StopAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore() =>
        await _connection.DisposeAsync().ConfigureAwait(false);

    public abstract Task OnMessage(ChannelMessage @event);
    public abstract Task OnNotification(ServerNotification @event);
    public abstract Task OnUserConnected(UserConnected @event);
    public abstract Task OnUserDisconnected(UserDisconnected @event);
    public abstract Task OnChannelMemberJoined(ChannelMemberJoined @event);
    public abstract Task OnChannelMemberLeft(ChannelMemberLeft @event);

    public Task SendChannelMessage(string channelId, string contentType, byte[] content, CancellationToken cancellationToken = default)
        => _connection.InvokeAsync(nameof(SendChannelMessage), channelId, contentType, content, cancellationToken);
}
