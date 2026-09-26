using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.Identity.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResult IssueToken(
        ICurrentUserContext context,
        IReadOnlyCollection<string> permissions)
    {
        var issuer = _configuration["Jwt:Issuer"]
                     ?? throw new InvalidOperationException(
                         "JWT issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
                       ?? throw new InvalidOperationException(
                           "JWT audience is not configured.");

        var signingKey = _configuration["Jwt:SigningKey"]
                         ?? throw new InvalidOperationException(
                             "JWT signing key is not configured.");

        var expirationMinutes =
            _configuration.GetValue<int>("Jwt:ExpirationMinutes");

        if (expirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT expiration must be greater than zero.");
        }

        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                context.UserId.ToString()),

            new(
                "external_id",
                context.ExternalId),

            new(
                "user_role_id",
                context.ActiveUserRoleId.ToString()),

            new(
                "role_id",
                context.RoleId.ToString()),

            new(
                "portal",
                ((int)context.Portal).ToString()),

            new(
                "scope",
                ((int)context.ScopeType).ToString())
        };

        if (context.CenterId.HasValue)
        {
            claims.Add(
                new Claim(
                    "center_id",
                    context.CenterId.Value.ToString()));
        }

        foreach (var permission in permissions)
        {
            claims.Add(
                new Claim(
                    "permission",
                    permission));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResult(
            accessToken,
            expiresAtUtc);
    }
}