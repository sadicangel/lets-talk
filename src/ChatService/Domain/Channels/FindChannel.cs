using LetsTalk.Chat.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Chat.Channels;

public union FindChannelResult(SuccessResult<Channel>, ChannelNotFoundResult);

public sealed record class FindChannelQuery(string ChannelId) : IRequest<FindChannelResult>;

public sealed class FindChannelQueryHandler(ChatDbContext dbContext) : IRequestHandler<FindChannelQuery, FindChannelResult>
{
    public async ValueTask<FindChannelResult> Handle(FindChannelQuery request, CancellationToken cancellationToken)
    {
        var channel = await dbContext.Channels.Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return new ChannelNotFoundResult();
        }

        return new SuccessResult<Channel>(channel);
    }
}
