using Domain.Identity;

namespace Application.Common.Interfaces;

public interface ICurrentUserContext
{
    Guid UserId { get; }

    string ExternalId { get; }

    Guid ActiveUserRoleId { get; }

    Guid RoleId { get; }

    PortalType Portal { get; }

    ScopeType ScopeType { get; }

    Guid? CenterId { get; }
}