using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Identity;

namespace Api.Authentication;

public sealed class JwtCurrentUserContext : ICurrentUserContext
{
    public Guid UserId { get; }

    public string ExternalId { get; }
    
    public bool HasElsaAccess { get; }

    public Guid ActiveUserRoleId { get; }

    public Guid RoleId { get; }

    public PortalType Portal { get; }

    public ScopeType ScopeType { get; }

    public Guid? CenterId { get; }

    public JwtCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException(
                "No authenticated user context is available.");

        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException(
                "The request is not authenticated.");
        }

        UserId = ParseGuidClaim(
            principal,
            JwtRegisteredClaimNames.Sub);

        ExternalId = GetRequiredClaim(
            principal,
            "external_id");
        
        HasElsaAccess = ParseOptionalBoolClaim(
            principal,
            "elsa_access");

        ActiveUserRoleId = ParseGuidClaim(
            principal,
            "user_role_id");

        RoleId = ParseGuidClaim(
            principal,
            "role_id");

        Portal = ParseEnumClaim<PortalType>(
            principal,
            "portal");

        ScopeType = ParseEnumClaim<ScopeType>(
            principal,
            "scope");

        CenterId = ParseOptionalGuidClaim(
            principal,
            "center_id");
    }

    private static string GetRequiredClaim(
        ClaimsPrincipal principal,
        string claimType)
    {
        var value = principal.FindFirstValue(claimType);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UnauthorizedAccessException(
                $"Required claim '{claimType}' is missing.");
        }

        return value;
    }

    private static Guid ParseGuidClaim(
        ClaimsPrincipal principal,
        string claimType)
    {
        var value = GetRequiredClaim(
            principal,
            claimType);

        if (!Guid.TryParse(value, out var result))
        {
            throw new UnauthorizedAccessException(
                $"Claim '{claimType}' is invalid.");
        }

        return result;
    }

    private static Guid? ParseOptionalGuidClaim(
        ClaimsPrincipal principal,
        string claimType)
    {
        var value = principal.FindFirstValue(claimType);

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Guid.TryParse(value, out var result))
        {
            throw new UnauthorizedAccessException(
                $"Claim '{claimType}' is invalid.");
        }

        return result;
    }

    private static TEnum ParseEnumClaim<TEnum>(
        ClaimsPrincipal principal,
        string claimType)
        where TEnum : struct, Enum
    {
        var value = GetRequiredClaim(
            principal,
            claimType);

        if (!int.TryParse(value, out var numericValue) ||
            !Enum.IsDefined(typeof(TEnum), numericValue))
        {
            throw new UnauthorizedAccessException(
                $"Claim '{claimType}' is invalid.");
        }

        return (TEnum)Enum.ToObject(
            typeof(TEnum),
            numericValue);
    }
    
    private static bool ParseOptionalBoolClaim(
        ClaimsPrincipal principal,
        string claimType)
    {
        var value = principal.FindFirstValue(claimType);

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!bool.TryParse(value, out var result))
        {
            throw new UnauthorizedAccessException(
                $"Claim '{claimType}' is invalid.");
        }

        return result;
    }
}