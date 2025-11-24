using LetsTalk.Chat.Entities;
using LetsTalk.Models;
using Mediator;
using Microsoft.Extensions.Logging;
using OneOf;

namespace LetsTalk.Chat.Channels.CreateChannel;

public sealed record class CreateChannelCommand(UserIdentity User, string Name, string? Description) : IRequest<CreateChannelResult>;

[GenerateOneOf]
public sealed partial class CreateChannelResult : OneOfBase<CreateChannelResult.Success, CreateChannelResult.Invalid>
{
    public readonly record struct Success(Channel Channel);

    public readonly record struct Invalid(IReadOnlyDictionary<string, string[]> Errors);
}

public sealed class CreateChannelCommandHandler(ChatDbContext dbContext, TimeProvider timeProvider, ILogger<CreateChannelCommandHandler> logger) : IRequestHandler<CreateChannelCommand, CreateChannelResult>
{
    public async ValueTask<CreateChannelResult> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new CreateChannelResult.Invalid(new Dictionary<string, string[]> { [nameof(request.Name)] = ["Channel name cannot be null or whitespace"] });
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

        return new CreateChannelResult.Success(channel);
    }
}
