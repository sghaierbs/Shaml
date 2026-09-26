using Domain.Identity;

namespace Application.Identity.GetUserRoles;

public sealed record UserRoleResult(
    Guid UserRoleId,
    Guid RoleId,
    string RoleCode,
    string RoleName,
    PortalType Portal,
    ScopeType ScopeType,
    Guid? CenterId,
    string? CenterName,
    bool IsDefault);