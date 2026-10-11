using System.Diagnostics;
using LetsTalk.Chat.Channels;
using LetsTalk.Chat.Entities;
using LetsTalk.Services;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LetsTalk.Chat;

public static class ChannelEndpoints
{
    public static IEndpointConventionBuilder MapChannelEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("api/channels");

        api.MapGet("/", GetChannels);
        api.MapGet("/{channelId}", GetChannel);
        api.MapPost("/", CreateChannel);
        api.MapPut("/{channelId}", UpdateChannel);
        api.MapDelete("/{channelId}", DeleteChannel);

        api.MapPatch("/{channelId}/join", JoinChannel);
        api.MapPatch("/{channelId}/leave", LeaveChannel);

        return api;
    }

    private static async Task<Ok<ChannelListResponse>> GetChannels(string? userId, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetChannelsQuery(userId), cancellationToken) switch
        {
            SuccessResult<IReadOnlyList<Channel>>(var channels) => TypedResults.Ok(new ChannelListResponse([.. channels.Select(channel => new ChannelResponse(channel.Id, channel.Name, channel.Description, [.. channel.Members.Select(x => x.UserId)]))])),
            SuccessResult => throw new UnreachableException(), // TODO: This never happens, but I guess we can't have union types that only return one case?
            _ => throw new UnreachableException()
        };
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound>> GetChannel(string channelId, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new FindChannelQuery(channelId), cancellationToken) switch
        {
            SuccessResult<Channel>(var channel) => TypedResults.Ok(new ChannelResponse(channel.Id, channel.Name, channel.Description, [.. channel.Members.Select(x => x.UserId)])),
            ChannelNotFoundResult => TypedResults.NotFound(),
            _ => throw new UnreachableException()
        };
    }

    private static async Task<Results<Created<ChannelResponse>, ValidationProblem>> CreateChannel(ChannelRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateChannelCommand(context.User.UserIdentity, request.Name, request.Description), cancellationToken) switch
        {
            SuccessResult<Channel>(var channel) => TypedResults.Created($"/api/channels/{channel.Id}", new ChannelResponse(channel.Id, channel.Name, channel.Description, [.. channel.Members.Select(x => x.UserId)])),
            InvalidResult(var errors) => TypedResults.ValidationProblem(errors.ToDictionary()),
            _ => throw new UnreachableException()
        };
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound, UnauthorizedHttpResult, ValidationProblem>> UpdateChannel(string channelId, ChannelRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateChannelCommand(context.User.UserIdentity, channelId, request.Name, request.Description), cancellationToken) switch
        {
            SuccessResult<Channel>(var channel) => TypedResults.Ok(new ChannelResponse(channel.Id, channel.Name, channel.Description, [.. channel.Members.Select(x => x.UserId)])),
            InvalidResult(var errors) => TypedResults.ValidationProblem(errors.ToDictionary()),
            ChannelNotFoundResult => TypedResults.NotFound(),
            UnauthorizedResult => TypedResults.Unauthorized(),
            _ => throw new UnreachableException()
        };
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> DeleteChannel(string channelId, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        return await mediator.Send(new DeleteChannelCommand(context.User.UserIdentity, channelId), cancellationToken) switch
        {
            SuccessResult => TypedResults.NoContent(),
            ChannelNotFoundResult => TypedResults.NotFound(),
            UnauthorizedResult => TypedResults.Unauthorized(),
            _ => throw new UnreachableException()
        };
    }

    private static async Task<Results<Ok, NotFound>> JoinChannel(string channelId, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        return await mediator.Send(new JoinChannelCommand(context.User.UserIdentity, channelId), cancellationToken) switch
        {
            SuccessResult => TypedResults.Ok(),
            ChannelNotFoundResult => TypedResults.NotFound(),
            UserAlreadyMemberResult => TypedResults.Ok(),
            _ => throw new UnreachableException()
        };
    }

    private static async Task<Results<Ok, NotFound>> LeaveChannel(string channelId, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        return await mediator.Send(new LeaveChannelCommand(context.User.UserIdentity, channelId), cancellationToken) switch
        {
            SuccessResult => TypedResults.Ok(),
            ChannelNotFoundResult => TypedResults.NotFound(),
            UserNotMemberResult => TypedResults.Ok(),
            _ => throw new UnreachableException()
        };
    }
}
