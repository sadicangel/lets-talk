using LetsTalk.Chat.Channels.CreateChannel;
using LetsTalk.Chat.Channels.DeleteChannel;
using LetsTalk.Chat.Channels.FindChannel;
using LetsTalk.Chat.Channels.GetChannels;
using LetsTalk.Chat.Channels.JoinChannel;
using LetsTalk.Chat.Channels.LeaveChannel;
using LetsTalk.Chat.Channels.UpdateChannel;
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
        var result = await mediator.Send(new GetChannelsQuery(userId), cancellationToken);
        return result.Match<Ok<ChannelListResponse>>(success => TypedResults.Ok(new ChannelListResponse([.. success.Channels.Select(channel => new ChannelResponse(channel.Id, channel.Name, channel.Description, [.. channel.Members.Select(x => x.UserId)]))])));
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound>> GetChannel(string channelId, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new FindChannelQuery(channelId), cancellationToken);
        return result.Match<Results<Ok<ChannelResponse>, NotFound>>(
            success => TypedResults.Ok(new ChannelResponse(success.Channel.Id, success.Channel.Name, success.Channel.Description, [.. success.Channel.Members.Select(x => x.UserId)])),
            channelNotFound => TypedResults.NotFound());
    }

    private static async Task<Results<Created<ChannelResponse>, ValidationProblem>> CreateChannel(ChannelRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateChannelCommand(context.User.UserIdentity, request.Name, request.Description), cancellationToken);
        return result.Match<Results<Created<ChannelResponse>, ValidationProblem>>(
            success => TypedResults.Created($"/api/channels/{success.Channel.Id}", new ChannelResponse(success.Channel.Id, success.Channel.Name, success.Channel.Description, [.. success.Channel.Members.Select(x => x.UserId)])),
            invalid => TypedResults.ValidationProblem(invalid.Errors));
    }

    private static async Task<Results<Ok<ChannelResponse>, NotFound, UnauthorizedHttpResult, ValidationProblem>> UpdateChannel(string channelId, ChannelRequest request, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateChannelCommand(context.User.UserIdentity, channelId, request.Name, request.Description), cancellationToken);
        return result.Match<Results<Ok<ChannelResponse>, NotFound, UnauthorizedHttpResult, ValidationProblem>>(
            success => TypedResults.Ok(new ChannelResponse(success.Channel.Id, success.Channel.Name, success.Channel.Description, [.. success.Channel.Members.Select(x => x.UserId)])),
            invalid => TypedResults.ValidationProblem(invalid.Errors),
            channelNotFound => TypedResults.NotFound(),
            unauthorized => TypedResults.Unauthorized());
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> DeleteChannel(string channelId, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteChannelCommand(context.User.UserIdentity, channelId), cancellationToken);
        return result.Match<Results<NoContent, NotFound, UnauthorizedHttpResult>>(
            success => TypedResults.NoContent(),
            channelNotFound => TypedResults.NotFound(),
            unauthorized => TypedResults.Unauthorized());
    }

    private static async Task<Results<Ok, NotFound>> JoinChannel(string channelId, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new JoinChannelCommand(context.User.UserIdentity, channelId), cancellationToken);
        return result.Match<Results<Ok, NotFound>>(
            success => TypedResults.Ok(),
            channelNotFound => TypedResults.NotFound(),
            userAlreadyMember => TypedResults.Ok());
    }

    private static async Task<Results<Ok, NotFound>> LeaveChannel(string channelId, IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LeaveChannelCommand(context.User.UserIdentity, channelId), cancellationToken);
        return result.Match<Results<Ok, NotFound>>(
            success => TypedResults.Ok(),
            channelNotFound => TypedResults.NotFound(),
            userNotMember => TypedResults.Ok());
    }
}
