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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

        services.AddAuthorization();

        return services;
    }
}