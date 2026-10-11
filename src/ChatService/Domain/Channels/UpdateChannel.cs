using System.ComponentModel.DataAnnotations;
using LetsTalk.Chat.Entities;
using LetsTalk.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Channels;

public union UpdateChannelResult(SuccessResult<Channel>, InvalidResult, ChannelNotFoundResult, UnauthorizedResult);

public sealed record class UpdateChannelCommand(UserIdentity User, string ChannelId, string Name, string? Description) : IRequest<UpdateChannelResult>;

public sealed class UpdateChannelCommandHandler(ChatDbContext dbContext, ILogger<UpdateChannelCommandHandler> logger) : IRequestHandler<UpdateChannelCommand, UpdateChannelResult>
{
    public async ValueTask<UpdateChannelResult> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new InvalidResult([new ValidationResult("Channel name cannot be null or whitespace", [nameof(request.Name)])]);
        }

        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken: cancellationToken);
        if (channel is null)
        {
            return new ChannelNotFoundResult();
        }

        var user = channel.Members.Find(x => x.UserId == request.User.UserId);
        if (user is not { Role: >= ChannelRole.Moderator })
        {
            return new UnauthorizedResult();
        }

        channel.Name = request.Name;
        channel.Description = request.Description;

        dbContext.Channels.Update(channel);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Channel {ChannelId} updated by user {UserId}", channel.Id, request.User.UserId);

        return new SuccessResult<Channel>(channel);
    }
}
