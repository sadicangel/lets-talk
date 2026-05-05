using LetsTalk.Chat.Entities;
using LetsTalk.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;

namespace LetsTalk.Chat.Channels.UpdateChannel;

public sealed record class UpdateChannelCommand(UserIdentity User, string ChannelId, string Name, string? Description) : IRequest<UpdateChannelResult>;

[GenerateOneOf]
public sealed partial class UpdateChannelResult : OneOfBase<UpdateChannelResult.Success, UpdateChannelResult.Invalid, UpdateChannelResult.ChannelNotFound, UpdateChannelResult.Unauthorized>
{
    public readonly record struct Success(Channel Channel);

    public readonly record struct Invalid(IReadOnlyDictionary<string, string[]> Errors);

    public struct ChannelNotFound;

    public struct Unauthorized;
}

public sealed class UpdateChannelCommandHandler(ChatDbContext dbContext, ILogger<UpdateChannelCommandHandler> logger) : IRequestHandler<UpdateChannelCommand, UpdateChannelResult>
{
    public async ValueTask<UpdateChannelResult> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new UpdateChannelResult.Invalid(new Dictionary<string, string[]> { [nameof(request.Name)] = ["Channel name cannot be null or whitespace"] });
        }

        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken: cancellationToken);
        if (channel is null)
        {
            return new UpdateChannelResult.ChannelNotFound();
        }

        var user = channel.Members.Find(x => x.UserId == request.User.UserId);
        if (user is not { Role: >= ChannelRole.Moderator })
        {
            return new UpdateChannelResult.Unauthorized();
        }

        channel.Name = request.Name;
        channel.Description = request.Description;

        dbContext.Channels.Update(channel);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Channel {ChannelId} updated by user {UserId}", channel.Id, request.User.UserId);

        return new UpdateChannelResult.Success(channel);
    }
}
