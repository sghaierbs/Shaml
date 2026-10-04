using System.Text;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddShamlAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"]
                     ?? throw new InvalidOperationException(
                         "JWT issuer is not configured.");

        var audience = configuration["Jwt:Audience"]
                       ?? throw new InvalidOperationException(
                           "JWT audience is not configured.");

        var signingKey = configuration["Jwt:SigningKey"]
                         ?? throw new InvalidOperationException(
                             "JWT signing key is not configured.");

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserContext, JwtCurrentUserContext>();

        // Register Shaml JWT authentication as a named scheme.
        // Do NOT make it the global default because Elsa uses "Bearer".
        services
            .AddAuthentication()
            .AddJwtBearer(
                AuthenticationSchemes.Shaml,
                options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = issuer,

                            ValidateAudience = true,
                            ValidAudience = audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(signingKey)),

                            ValidateLifetime = true,

                            ClockSkew = TimeSpan.FromSeconds(30)
                        };
                });

        // Shaml business-user authorization.
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.ShamlUser,
                policy =>
                {
                    policy.AddAuthenticationSchemes(
                        AuthenticationSchemes.Shaml);

                    policy.RequireAuthenticatedUser();
                });
        });

        return services;
    }
}

public static class AuthenticationSchemes
{
    public const string Shaml = "ShamlBearer";
}

public static class AuthorizationPolicies
{
    public const string ShamlUser = "ShamlUser";
}