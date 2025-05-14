using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using LetsTalk.IdentityService.WebApi.Services;
using LetsTalk.Shared.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LetsTalk.IdentityService.WebApi;

public static class IdentityEndpoints
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute s_emailAddressAttribute = new();

    public static IEndpointConventionBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("api");
        api.MapPost("/register", Register);
        api.MapPost("/login", Login);
        api.MapPost("/refresh", Refresh);
        return api;
    }

    private static async Task<Results<Ok, ValidationProblem>> Register(
        RegisterRequest registration,
        HttpContext context,
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore)
    {
        if (string.IsNullOrEmpty(registration.UserName))
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidUserName(registration.UserName)));

        if (!s_emailAddressAttribute.IsValid(registration.Email))
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(registration.Email)));

        var user = new IdentityUser();
        await userStore.SetUserNameAsync(user, registration.UserName, CancellationToken.None);
        await ((IUserEmailStore<IdentityUser>)userStore).SetEmailAsync(user, registration.Email, CancellationToken.None);

        var result = await userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
            return CreateValidationProblem(result);

        await SendConfirmationEmailAsync(user, userManager, context, registration.Email);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Login(
        LoginRequest login,
        SignInManager<IdentityUser> signInManager,
        JwtBearerTokenProvider tokenProvider)
    {
        signInManager.AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;

        var user = await signInManager.UserManager.FindByNameAsync(login.UserName);
        if (user is null)
            return TypedResults.Problem(SignInResult.Failed.ToString(), statusCode: StatusCodes.Status401Unauthorized);

        var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);

        var (accessToken, refreshToken, expiresIn) = await tokenProvider.GenerateAsync(user);

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return TypedResults.Ok(new AccessTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expiresIn,
        });
    }

    private static async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh(
        RefreshRequest refreshRequest,
        SignInManager<IdentityUser> signInManager,
        IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
        TimeProvider timeProvider)
    {
        var refreshTokenProtector = bearerTokenOptions.Get(JwtBearerDefaults.AuthenticationScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not IdentityUser user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: JwtBearerDefaults.AuthenticationScheme);
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private static async Task SendConfirmationEmailAsync(IdentityUser user, UserManager<IdentityUser> userManager, HttpContext context, string email, bool isChange = false)
    {
        _ = user;
        _ = userManager;
        _ = context;
        _ = email;
        _ = isChange;
        await Task.CompletedTask;

        //if (confirmEmailEndpointName is null)
        //{
        //    throw new NotSupportedException("No email confirmation endpoint was registered!");
        //}

        //var code = isChange
        //    ? await userManager.GenerateChangeEmailTokenAsync(user, email)
        //    : await userManager.GenerateEmailConfirmationTokenAsync(user);
        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        //var userId = await userManager.GetUserIdAsync(user);
        //var routeValues = new RouteValueDictionary()
        //{
        //    ["userId"] = userId,
        //    ["code"] = code,
        //};

        //if (isChange)
        //{
        //    // This is validated by the /confirmEmail endpoint on change.
        //    routeValues.Add("changedEmail", email);
        //}

        //var confirmEmailUrl = linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
        //    ?? throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");

        //await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
    }
}
