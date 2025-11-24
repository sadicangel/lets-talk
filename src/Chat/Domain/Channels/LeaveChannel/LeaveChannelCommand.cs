using LetsTalk.Chat.Services;
using LetsTalk.Events;
using LetsTalk.Models;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;

namespace LetsTalk.Chat.Channels.LeaveChannel;

public sealed record class LeaveChannelCommand(UserIdentity User, string ChannelId) : IRequest<LeaveChannelResult>;

[GenerateOneOf]
public partial class LeaveChannelResult :
    OneOfBase<LeaveChannelResult.Success, LeaveChannelResult.ChannelNotFound, LeaveChannelResult.UserNotMember>
{
    public struct Success;

    public struct ChannelNotFound;

    public struct UserNotMember;
}

public sealed class LeaveChannelCommandHandler(ChatDbContext dbContext, IHubContext<ChatHub, IChatHubClient> hubContext, ILogger<LeaveChannelCommandHandler> logger) : IRequestHandler<LeaveChannelCommand, LeaveChannelResult>
{
    public async ValueTask<LeaveChannelResult> Handle(LeaveChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return new LeaveChannelResult.ChannelNotFound();
        }

        var member = channel.Members.Find(x => x.UserId == request.User.UserId);
        if (member is null)
        {
            return new LeaveChannelResult.UserNotMember();
        }

        channel.Members.Remove(member);

        await dbContext.SaveChangesAsync(cancellationToken);

        await hubContext.Clients.Group(request.ChannelId).OnChannelMemberLeft(new ChannelMemberLeft(channel.ToIdentity(), request.User));

        logger.LogInformation("User {UserId} left channel {ChannelId}", request.User.UserId, request.ChannelId);

        return new LeaveChannelResult.Success();
    }
}
