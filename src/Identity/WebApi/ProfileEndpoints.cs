using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LetsTalk.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace LetsTalk.Identity;

public static class ProfileEndpoints
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute s_emailAddressAttribute = new();

    public static IEndpointConventionBuilder MapProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("api").RequireAuthorization();
        api.MapGet("/profile", GetProfile);
        api.MapPut("/profile", UpdateProfile);
        return api;
    }


    private static Results<Ok<UserProfileResponse>, UnauthorizedHttpResult> GetProfile(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return TypedResults.Unauthorized();

        var user = principal.UserIdentity;

        return TypedResults.Ok(new UserProfileResponse(user.UserId, user.UserName, user.Email, user.AvatarUrl));
    }

    private static async Task<Results<Ok<UserProfileResponse>, UnauthorizedHttpResult, NotFound, BadRequest>> UpdateProfile(UserProfileRequest request, ClaimsPrincipal principal, UserManager<AppUser> userManager)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return TypedResults.Unauthorized();

        var user = await userManager.FindByIdAsync(principal.UserId);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        if (!s_emailAddressAttribute.IsValid(request.Email))
        {
            return TypedResults.BadRequest();
        }

        user.Email = request.Email;
        user.AvatarUrl = request.AvatarUrl;

        return TypedResults.Ok(new UserProfileResponse(user.Id, user.UserName!, user.Email, user.AvatarUrl));
    }
}
