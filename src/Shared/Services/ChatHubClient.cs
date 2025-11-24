using System.Collections.Immutable;
using LetsTalk.Events;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Services;

public class ChatHubClient : IChatHubClient, IChatHubServer, IAsyncDisposable
{
    private readonly HubConnection _connection;
    private readonly ILogger<ChatHubClient> _logger;
    private readonly ImmutableArray<IDisposable> _subscriptions;

    public ChatHubClient(HubConnection connection, ILogger<ChatHubClient> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger;

        _subscriptions =
        [
            _connection.On<Message>(nameof(OnMessage), OnMessage),
            _connection.On<Notification>(nameof(OnNotification), OnNotification),
            _connection.On<UserConnected>(nameof(OnUserConnected), OnUserConnected),
            _connection.On<UserDisconnected>(nameof(OnUserDisconnected), OnUserDisconnected),
            _connection.On<ChannelMemberJoined>(nameof(OnChannelMemberJoined), OnChannelMemberJoined),
            _connection.On<ChannelMemberLeft>(nameof(OnChannelMemberLeft), OnChannelMemberLeft),
        ];
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default) => _connection.StartAsync(cancellationToken);

    public Task DisconnectAsync(CancellationToken cancellationToken = default) => _connection.StopAsync(cancellationToken);

    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();

        await _connection.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    public virtual Task OnMessage(Message @event)
    {
        _logger.LogInformation("Received event {@Event}", @event);
        return Task.CompletedTask;
    }

    public virtual Task OnNotification(Notification @event)
    {
        _logger.LogInformation("Received event {@Event}", @event);
        return Task.CompletedTask;
    }

    public virtual Task OnUserConnected(UserConnected @event)
    {
        _logger.LogInformation("Received event {@Event}", @event);
        return Task.CompletedTask;
    }

    public virtual Task OnUserDisconnected(UserDisconnected @event)
    {
        _logger.LogInformation("Received event {@Event}", @event);
        return Task.CompletedTask;
    }

    public virtual Task OnChannelMemberJoined(ChannelMemberJoined @event)
    {
        _logger.LogInformation("Received event {@Event}", @event);
        return Task.CompletedTask;
    }

    public virtual Task OnChannelMemberLeft(ChannelMemberLeft @event)
    {
        _logger.LogInformation("Received event {@Event}", @event);
        return Task.CompletedTask;
    }

    public Task SendMessage(string channelId, string contentType, byte[] content, CancellationToken cancellationToken = default)
        => _connection.InvokeAsync(nameof(SendMessage), channelId, contentType, content, cancellationToken);
}
