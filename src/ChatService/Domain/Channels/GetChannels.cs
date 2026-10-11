using LetsTalk.Chat.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Chat.Channels;

public union GetChannelsResult(SuccessResult<IReadOnlyList<Channel>>, SuccessResult);

public sealed record class GetChannelsQuery(string? UserId) : IRequest<GetChannelsResult>;

public sealed class GetChannelsQueryHandler(ChatDbContext dbContext) : IRequestHandler<GetChannelsQuery, GetChannelsResult>
{
    public async ValueTask<GetChannelsResult> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        var channels = string.IsNullOrWhiteSpace(request.UserId)
            ? await dbContext.Channels
                .Include(x => x.Members)
                .AsNoTracking()
                .ToListAsync(cancellationToken)
            : await dbContext.Members
                .Include(x => x.Channel)
                .AsNoTracking()
                .Where(m => m.UserId == request.UserId && m.Status == ChannelMembershipStatus.Active)
                .Select(m => m.Channel)
                .ToListAsync(cancellationToken);

        return new SuccessResult<IReadOnlyList<Channel>>(channels);
    }
}
