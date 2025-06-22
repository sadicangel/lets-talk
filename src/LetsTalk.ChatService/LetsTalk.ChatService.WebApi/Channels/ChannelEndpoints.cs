using LetsTalk.ChatService.Domain;
using LetsTalk.ChatService.Domain.Entities;
using LetsTalk.ChatService.WebApi.Hubs;
using LetsTalk.Shared;
using LetsTalk.Shared.Events;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.ChatService.WebApi.Channels;

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

        api.MapGet("/channels/{channelId}/join", JoinChannel);
        api.MapGet("/channels/{channelId}/leave", LeaveChannel);

        return api;
    }

    private static async Task<Ok<ChannelListResponse>> GetChannels(string? userId, ChatDbContext dbContext)
    {
        var channels = string.IsNullOrWhiteSpace(userId)
            ? await dbContext.Channels.AsNoTracking().Select(c => c.Id).ToArrayAsync()
            : await dbContext.Members.AsNoTracking().Where(m => m.UserId == userId).Select(m => m.ChannelId).ToArrayAsync();

        return TypedResults.Ok(new ChannelListResponse(channels));
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound>> GetChannel(string channelId, ChatDbContext dbContext)
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
        ChatDbContext dbContext,
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
        ChatDbContext dbContext,
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
        ChatDbContext dbContext,
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

    private static async Task<Results<Ok, NotFound>> JoinChannel(
        string channelId,
        ChatDbContext dbContext,
        HttpContext httpContext,
        IHubContext<ChatHub, ILetsTalkClient> hubContext)
    {
        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
        {
            return TypedResults.NotFound();
        }

        var member = channel.Members.Find(x => x.UserId == httpContext.User.GetUserId());
        if (member is not null)
        {
            // Already a member, no need to join again.
            return TypedResults.Ok();
        }

        channel.Members.Add(new ChannelMember
        {
            ChannelId = channel.Id,
            UserId = httpContext.User.GetUserId(),
            MemberSince = TimeProvider.System.GetUtcNow(),
            LastSeenAt = TimeProvider.System.GetUtcNow(),
            Role = ChannelRole.Member,
            Status = ChannelMembershipStatus.Active,
            InvitedByUserId = null
        });

        await dbContext.SaveChangesAsync();

        await hubContext.Clients.Group(channelId).OnChannelMemberJoined(
            new ChannelMemberJoined(channel.ToIdentity(), httpContext.User.GetUserIdentity()));

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> LeaveChannel(
        string channelId,
        ChatDbContext dbContext,
        HttpContext httpContext,
        IHubContext<ChatHub, ILetsTalkClient> hubContext)
    {
        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
        {
            return TypedResults.NotFound();
        }

        var member = channel.Members.Find(x => x.UserId == httpContext.User.GetUserId());
        if (member is null)
        {
            // Not a member, no need to remove.
            return TypedResults.Ok();
        }

        channel.Members.Remove(member);

        await dbContext.SaveChangesAsync();

        await hubContext.Clients.Group(channelId).OnChannelMemberLeft(
            new ChannelMemberLeft(channel.ToIdentity(), httpContext.User.GetUserIdentity()));

        return TypedResults.Ok();
    }
}
