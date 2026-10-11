using LetsTalk.Chat.Entities;
using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Models;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Channels;

public union JoinChannelResult(SuccessResult, ChannelNotFoundResult, UserAlreadyMemberResult);

public sealed record class JoinChannelCommand(UserIdentity User, string ChannelId) : IRequest<JoinChannelResult>;

public sealed class JoinChannelCommandHandler(ChatDbContext dbContext, ConnectionManager connectionManager, IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<JoinChannelCommandHandler> logger) : IRequestHandler<JoinChannelCommand, JoinChannelResult>
{
    public async ValueTask<JoinChannelResult> Handle(JoinChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return new ChannelNotFoundResult();
        }

        var member = channel.Members.Find(x => x.UserId == request.User.UserId);
        if (member is not null)
        {
            if (member.Status != ChannelMembershipStatus.Active)
            {
                member.Status = ChannelMembershipStatus.Active;
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            await AddActiveConnectionsToGroup(request.User.UserId, request.ChannelId, cancellationToken);
            return new UserAlreadyMemberResult();
        }

        channel.Members.Add(
            new ChannelMember
            {
                ChannelId = channel.Id,
                UserId = request.User.UserId,
                MemberSince = TimeProvider.System.GetUtcNow(),
                LastSeenAt = TimeProvider.System.GetUtcNow(),
                Role = ChannelRole.Member,
                Status = ChannelMembershipStatus.Active,
                InvitedByUserId = null
            });

        await dbContext.SaveChangesAsync(cancellationToken);
        await AddActiveConnectionsToGroup(request.User.UserId, request.ChannelId, cancellationToken);

        await hubContext.Clients.Group(request.ChannelId).OnChannelMemberJoined(new ChannelMemberJoined(channel.ToIdentity(), request.User));

        logger.LogInformation("User {UserId} joined channel {ChannelId}", request.User.UserId, request.ChannelId);

        return new SuccessResult();
    }

    private async Task AddActiveConnectionsToGroup(string userId, string channelId, CancellationToken cancellationToken)
    {
        foreach (var connectionId in connectionManager.GetConnections(userId))
            await hubContext.Groups.AddToGroupAsync(connectionId, channelId, cancellationToken);
    }
}
