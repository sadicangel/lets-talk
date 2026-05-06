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


    private static async Task<Results<Ok<UserProfileResponse>, UnauthorizedHttpResult, NotFound>> GetProfile(ClaimsPrincipal principal, UserManager<AppUser> userManager)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return TypedResults.Unauthorized();

        var user = await userManager.FindByIdAsync(principal.UserId);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new UserProfileResponse(user.Id, user.UserName!, user.Email!, user.AvatarUrl));
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

        var emailResult = await userManager.SetEmailAsync(user, request.Email);
        if (!emailResult.Succeeded)
        {
            return TypedResults.BadRequest();
        }

        user.AvatarUrl = request.AvatarUrl;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(new UserProfileResponse(user.Id, user.UserName!, user.Email!, user.AvatarUrl));
    }
}
