using System.ComponentModel.DataAnnotations;
using LetsTalk.Chat.Entities;
using LetsTalk.Models;
using Mediator;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Chat.Channels;

public union CreateChannelResult(SuccessResult<Channel>, InvalidResult);

public sealed record class CreateChannelCommand(UserIdentity User, string Name, string? Description) : IRequest<CreateChannelResult>;

public sealed class CreateChannelCommandHandler(ChatDbContext dbContext, TimeProvider timeProvider, ILogger<CreateChannelCommandHandler> logger) : IRequestHandler<CreateChannelCommand, CreateChannelResult>
{
    public async ValueTask<CreateChannelResult> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new InvalidResult(new ValidationResult("Channel name cannot be null or whitespace", [nameof(request.Name)]));
        }

        var utcNow = timeProvider.GetUtcNow();
        var channel = new Channel
        {
            Id = null!,
            Name = request.Name,
            Description = request.Description,
            Members =
            [
                new ChannelMember
                {
                    ChannelId = null!,
                    UserId = request.User.UserId,
                    MemberSince = utcNow,
                    LastSeenAt = utcNow,
                    Role = ChannelRole.Admin,
                    Status = ChannelMembershipStatus.Active,
                    InvitedByUserId = null
                }
            ]
        };

        dbContext.Channels.Add(channel);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created new channel {ChannelId} named {ChannelName} by user {UserId}", channel.Id, channel.Name, request.User.UserId);

        return new SuccessResult<Channel>(channel);
    }
}
