#pragma warning disable IDE0130 // Namespace does not match folder structure
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class LetsTalkExtensions
{
    public static AuthenticationBuilder AddLetsTalkJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        return builder.AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Issuer"]!,
                ValidateAudience = false,
                ValidAudience = configuration["Audience"]!,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["SecurityKey"]!)),
            };
        });
    }
}
