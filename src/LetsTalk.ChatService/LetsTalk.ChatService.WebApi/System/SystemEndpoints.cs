using LetsTalk.ChatService.WebApi.Hubs;
using LetsTalk.Shared.Events;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.ChatService.WebApi.System;

public static class SystemEndpoints
{
    public static IEndpointConventionBuilder MapChannelEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("api/system");

        api.MapGet("/notification", SendNotification);

        return api;
    }

    private static async Task<Ok> SendNotification(NotificationRequest request, IHubContext<ChatHub, ILetsTalkClient> hubContext)
    {
        await hubContext.Clients.All.OnNotification(new Notification(request.ContentType, request.Content));

        return TypedResults.Ok();
    }
}
