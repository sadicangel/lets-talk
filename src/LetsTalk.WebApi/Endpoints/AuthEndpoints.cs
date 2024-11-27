
using System.Security.Claims;
using LetsTalk.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/auth");

        auth.MapGet("/users", async (LetsTalkDbContext dbContext) =>
            await dbContext.Users.Select(u => u.UserName).ToListAsync());

        auth.MapPost("/login", Login)
            .DisableAntiforgery();

        return endpoints;
    }

    private static async Task<Results<Ok, UnauthorizedHttpResult>> Login(
        [FromForm] string username,
        [FromForm] string password,
        HttpContext httpContext,
        PasswordHasher passwordHasher,
        LetsTalkDbContext dbContext)
    {
        // Validate the user credentials.
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) is PasswordVerificationResult.Failed)
        {
            return TypedResults.Unauthorized();
        }

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(user.GetIdentity(CookieAuthenticationDefaults.AuthenticationScheme)));

        return TypedResults.Ok();
    }
}
