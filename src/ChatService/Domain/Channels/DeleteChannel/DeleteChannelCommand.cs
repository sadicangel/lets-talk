using LetsTalk.Chat.Entities;
using LetsTalk.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;

namespace LetsTalk.Chat.Channels.DeleteChannel;

public sealed record class DeleteChannelCommand(UserIdentity User, string ChannelId) : IRequest<DeleteChannelResult>;

[GenerateOneOf]
public sealed partial class DeleteChannelResult : OneOfBase<DeleteChannelResult.Success, DeleteChannelResult.ChannelNotFound, DeleteChannelResult.Unauthorized>
{
    public struct Success;

    public struct ChannelNotFound;

    public struct Unauthorized;
}

public sealed class DeleteChannelCommandHandler(ChatDbContext dbContext, ILogger<DeleteChannelCommandHandler> logger) : IRequestHandler<DeleteChannelCommand, DeleteChannelResult>
{
    public async ValueTask<DeleteChannelResult> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return new DeleteChannelResult.ChannelNotFound();
        }

        var user = channel.Members.Find(x => x.UserId == request.User.UserId);
        if (user is not { Role: >= ChannelRole.Moderator })
        {
            return new DeleteChannelResult.Unauthorized();
        }

        dbContext.Channels.Remove(channel);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Channel {ChannelId} deleted by user {UserId}", request.ChannelId, request.User.UserId);

        return new DeleteChannelResult.Success();
    }
}
