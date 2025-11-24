using LetsTalk.Chat.Hub.ConnectUser;
using LetsTalk.Chat.Hub.DisconnectUser;
using LetsTalk.Chat.Hub.SendMessage;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Chat.Services;

public sealed class ChatHub(IMediator mediator) : Hub<IChatHubClient>, IChatHubServer
{
    public override async Task OnConnectedAsync() =>
        await mediator.Send(new ConnectUserCommand(Context.User.UserIdentity, Context.ConnectionId));

    public override async Task OnDisconnectedAsync(Exception? exception) =>
        await mediator.Send(new DisconnectUserCommand(Context.User.UserIdentity, Context.ConnectionId));

    async Task IChatHubServer.SendMessage(string channelId, string contentType, byte[] content, CancellationToken cancellationToken) =>
        await SendMessage(channelId, contentType, content);

    public async Task SendMessage(string channelId, string contentType, byte[] content) =>
        await mediator.Send(new SendMessageCommand(Context.User.UserIdentity, channelId, contentType, content), Context.ConnectionAborted);
}
