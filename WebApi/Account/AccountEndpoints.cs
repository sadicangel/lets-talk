using LetsTalk.Account.Dtos;
using LetsTalk.Responses;
using LetsTalk.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LetsTalk.Account;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var account = endpoints.MapGroup("account");
        account.MapIdentityApi<User>();
        account.MapGet("profile", GetUserProfile).RequireAuthorization();
        account.MapGet("channels", GetUserChannels).RequireAuthorization();
        return endpoints;
    }

    private static async Task<Results<UnauthorizedHttpResult, NotFound, Ok<UserProfileResponse>>> GetUserProfile(
        ClaimsPrincipal principal,
        AppDbContext dbContext)
    {
        var user = await dbContext.Users
            .Include(x => x.Channels)
            .SingleOrDefaultAsync(x => x.Id == principal.FindFirstValue(ClaimTypes.NameIdentifier));
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(new UserProfileResponse(user));
    }

    private static async Task<Results<UnauthorizedHttpResult, NotFound, Ok<UserChannelListResponse>>> GetUserChannels(
        ClaimsPrincipal principal,
        UserManager<User> manager,
        AppDbContext dbContext)
    {
        var channels = await dbContext.UserChannels
            .Include(x => x.Channel)
            .Where(x => x.UserId == principal.FindFirstValue(ClaimTypes.NameIdentifier))
            .Select(x => x.Channel)
            .ToListAsync();

        return TypedResults.Ok(new UserChannelListResponse(channels));
    }
}
