using LetsTalk.Events;
using LetsTalk.Responses;
using LetsTalk.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LetsTalk.Channels;

public static class ChannelEndpoints
{
    public static IEndpointRouteBuilder MapChannelEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var channels = endpoints.MapGroup("channels");
        channels.MapGet("", GetChannelList).WithName(nameof(GetChannelList));
        channels.MapGet("{channelId}", GetChannelById).WithName(nameof(GetChannelById));
        channels.MapPost("", CreateChannel).WithName(nameof(CreateChannel));
        channels.MapPut("{channelId}", UpdateChannel).WithName(nameof(UpdateChannel));
        channels.MapDelete("{channelId}", DeleteChannel).WithName(nameof(DeleteChannel));
        return endpoints;
    }

    private static async Task<Results<UnauthorizedHttpResult, Ok<ChannelListResponse>>> GetChannelList(
        string? after,
        AppDbContext dbContext)
    {
        var channelsQuery = dbContext.Channels.AsQueryable();
        if (after is not null)
            channelsQuery = channelsQuery.Where(x => after.CompareTo(x.Id) > 0);

        var channels = await channelsQuery
            .Take(50)
            .Select(x => new ChannelProfile(x))
            .ToListAsync();
        return TypedResults.Ok(new ChannelListResponse(channels));
    }

    private static async Task<Results<UnauthorizedHttpResult, NotFound, Ok<ChannelProfile>>> GetChannelById(
        string channelId,
        AppDbContext dbContext)
    {
        var channel = await dbContext.Channels.SingleOrDefaultAsync(x => x.Id == channelId);
        return channel is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(new ChannelProfile(channel));
    }

    private static async Task<Results<UnauthorizedHttpResult, ValidationProblem, CreatedAtRoute<ChannelProfile>>> CreateChannel(
        CreateChannelRequest request,
        ClaimsPrincipal principal,
        AppDbContext dbContext,
        IHubContext<AppHub, IAppHubClient> hubContext,
        HubConnectionManager hubConnectionManager)
    {
        if (request.HasErrors(out var validation))
            return TypedResults.ValidationProblem(validation.MessageMap.ToDictionary(x => x.Key, x => x.Value.ToArray()));

        var user = await dbContext.Users
            .Include(x => x.Channels)
            .SingleAsync(x => x.Id == principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var channelId = Guid.NewGuid().ToString();
        var channel = new Channel
        {
            Id = channelId,
            DisplayName = request.ChannelName,
            Icon = request.ChannelIcon ?? $"https://api.dicebear.com/7.x/shapes/svg?seed={channelId}",
            Admin = user,
            AdminId = user.Id,
            Participants = [user]
        };
        user.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        foreach (var connectionId in hubConnectionManager.GetConnections(user.Id))
            await hubContext.Groups.AddToGroupAsync(connectionId, channel.Id);
        await hubContext.Clients.All.OnChannelCreated(new ChannelCreated(channel));

        return TypedResults.CreatedAtRoute(new ChannelProfile(channel), nameof(GetChannelById), new { channelId });
    }

    private static async Task<Results<UnauthorizedHttpResult, NotFound, ValidationProblem, Ok>> UpdateChannel(
        string channelId,
        UpdateChannelRequest request,
        ClaimsPrincipal principal,
        AppDbContext dbContext,
        IHubContext<AppHub, IAppHubClient> hubContext,
        HubConnectionManager hubConnectionManager)
    {
        if (request.HasErrors(out var validation))
            return TypedResults.ValidationProblem(validation.MessageMap.ToDictionary(x => x.Key, x => x.Value.ToArray()));

        var channel = await dbContext.Channels.SingleOrDefaultAsync(x => x.Id == channelId);
        if (channel is null)
            return TypedResults.NotFound();

        channel.DisplayName = request.ChannelName;
        channel.Icon = request.ChannelIcon;
        await dbContext.SaveChangesAsync();

        await hubContext.Clients.All.OnChannelUpdated(new ChannelUpdated(channel));

        return TypedResults.Ok();
    }

    private static async Task<Results<UnauthorizedHttpResult, NotFound, Ok>> DeleteChannel(
        string channelId,
        ClaimsPrincipal principal,
        AppDbContext dbContext,
        IHubContext<AppHub, IAppHubClient> hubContext,
        HubConnectionManager hubConnectionManager)
    {
        var channel = await dbContext.Channels
            .Include(x => x.Admin)
            .Include(x => x.Participants)
            .SingleOrDefaultAsync(x => x.Id == channelId);

        if (channel is null)
            return TypedResults.NotFound();

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (channel.Admin.Id != userId)
            return TypedResults.Unauthorized();

        dbContext.Channels.Remove(channel);

        await dbContext.SaveChangesAsync();

        await hubContext.Clients.All.OnChannelDeleted(new ChannelDeleted(channel));
        foreach (var connectionId in hubConnectionManager.GetConnections(userId))
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, channel.Id);

        return TypedResults.Ok();
    }
}