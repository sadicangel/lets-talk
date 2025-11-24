using LetsTalk.Chat.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LetsTalk.Chat.Channels.FindChannel;

public sealed record class FindChannelQuery(string ChannelId) : IRequest<FindChannelResult>;

[GenerateOneOf]
public sealed partial class FindChannelResult : OneOfBase<FindChannelResult.Success, FindChannelResult.ChannelNotFound>
{
    public readonly record struct Success(Channel Channel);

    public struct ChannelNotFound;
}

public sealed class FindChannelQueryHandler(ChatDbContext dbContext) : IRequestHandler<FindChannelQuery, FindChannelResult>
{
    public async ValueTask<FindChannelResult> Handle(FindChannelQuery request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return new FindChannelResult.ChannelNotFound();
        }

        return new FindChannelResult.Success(channel);
    }
}
