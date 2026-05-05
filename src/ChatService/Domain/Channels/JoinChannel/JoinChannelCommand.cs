using LetsTalk.Chat.Entities;
using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Models;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;

namespace LetsTalk.Chat.Channels.JoinChannel;

public sealed record class JoinChannelCommand(UserIdentity User, string ChannelId) : IRequest<JoinChannelResult>;

[GenerateOneOf]
public partial class JoinChannelResult :
    OneOfBase<JoinChannelResult.Success, JoinChannelResult.ChannelNotFound, JoinChannelResult.UserAlreadyMember>
{
    public struct Success;

    public struct ChannelNotFound;

    public struct UserAlreadyMember;
}

public sealed class JoinChannelCommandHandler(ChatDbContext dbContext, IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<JoinChannelCommandHandler> logger) : IRequestHandler<JoinChannelCommand, JoinChannelResult>
{
    public async ValueTask<JoinChannelResult> Handle(JoinChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return new JoinChannelResult.Success();
        }

        var member = channel.Members.Find(x => x.UserId == request.User.UserId);
        if (member is not null)
        {
            return new JoinChannelResult.UserAlreadyMember();
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

        await hubContext.Clients.Group(request.ChannelId).OnChannelMemberJoined(new ChannelMemberJoined(channel.ToIdentity(), request.User));

        logger.LogInformation("User {UserId} joined channel {ChannelId}", request.User.UserId, request.ChannelId);

        return new JoinChannelResult.Success();
    }
}
