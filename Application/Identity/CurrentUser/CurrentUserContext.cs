using Application.Common.Interfaces;
using Domain.Identity;

namespace Application.Identity.CurrentUser;
    
public sealed record CurrentUserContext(
    Guid UserId,
    string ExternalId,
    Guid ActiveUserRoleId,
    Guid RoleId,
    PortalType Portal,
    ScopeType ScopeType,
    Guid? CenterId): ICurrentUserContext;