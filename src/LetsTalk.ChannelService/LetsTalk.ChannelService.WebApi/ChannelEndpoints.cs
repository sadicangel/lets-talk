using LetsTalk.ChannelService.WebApi.Entities;
using LetsTalk.Shared;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.ChannelService.WebApi;

public static class ChannelEndpoints
{
    public static IEndpointConventionBuilder MapChannelEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("api");
        api.MapGet("/channels", GetChannels);
        api.MapGet("/channels/{channelId}", GetChannel);
        api.MapPost("/channels", CreateChannel);
        api.MapPut("/channels/{channelId}", UpdateChannel);
        api.MapDelete("/channels/{channelId}", DeleteChannel);
        return api;
    }

    private static async Task<Ok<ChannelListResponse>> GetChannels(string? userId, ChannelDbContext dbContext)
    {
        var channels = string.IsNullOrWhiteSpace(userId)
            ? await dbContext.Channels.AsNoTracking().Select(c => c.Id).ToArrayAsync()
            : await dbContext.Members.AsNoTracking().Where(m => m.UserId == userId).Select(m => m.ChannelId).ToArrayAsync();

        return TypedResults.Ok(new ChannelListResponse(channels));
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound>> GetChannel(string channelId, ChannelDbContext dbContext)
    {
        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
            return TypedResults.NotFound();

        var response = new ChannelResponse(
            channel.Id,
            channel.Name,
            channel.Description,
            [.. channel.Members.Select(x => x.UserId)]);

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Created<ChannelResponse>, ValidationProblem>> CreateChannel(
        ChannelRequest request,
        ChannelDbContext dbContext,
        TimeProvider timeProvider,
        HttpContext httpContext)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Name)] = ["Channel name cannot be null or whitespace"]
            });
        }

        var utcNow = timeProvider.GetUtcNow();
        var channel = new Channel
        {
            Id = null!,
            Name = request.Name,
            Description = request.Description,
            Members = [
                new ChannelMember {
                    ChannelId = null!,
                    UserId = httpContext.User.GetUserIdentity().UserId,
                    MemberSince = utcNow,
                    LastSeenAt = utcNow,
                    Role = ChannelRole.Admin,
                    Status = ChannelMembershipStatus.Active,
                    InvitedByUserId = null
            }]
        };

        dbContext.Channels.Add(channel);
        await dbContext.SaveChangesAsync();

        var response = new ChannelResponse(
            channel.Id,
            channel.Name,
            channel.Description,
            [.. channel.Members.Select(x => x.UserId)]);

        return TypedResults.Created($"/api/{channel.Id}", response);
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound, UnauthorizedHttpResult, ValidationProblem>> UpdateChannel(
        string channelId,
        [FromBody] ChannelRequest request,
        ChannelDbContext dbContext,
        HttpContext httpContext)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Name)] = ["Channel name cannot be null or whitespace"]
            });
        }

        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
        {
            return TypedResults.NotFound();
        }

        var user = channel.Members.Find(x => x.UserId == httpContext.User.GetUserId());
        if (user is not { Role: >= ChannelRole.Moderator })
        {
            return TypedResults.Unauthorized();
        }

        channel.Name = request.Name;
        channel.Description = request.Description;

        dbContext.Channels.Update(channel);
        await dbContext.SaveChangesAsync();

        var response = new ChannelResponse(
            channel.Id,
            channel.Name,
            channel.Description,
            [.. channel.Members.Select(x => x.UserId)]);

        return TypedResults.Ok(response);
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> DeleteChannel(
        string channelId,
        ChannelDbContext dbContext,
        HttpContext httpContext)
    {
        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
        {
            return TypedResults.NotFound();
        }

        var user = channel.Members.Find(x => x.UserId == httpContext.User.GetUserId());
        if (user is not { Role: >= ChannelRole.Moderator })
        {
            return TypedResults.Unauthorized();
        }

        dbContext.Channels.Remove(channel);
        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
