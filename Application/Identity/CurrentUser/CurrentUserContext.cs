using Application.Common.Interfaces;

namespace Application.Identity.CurrentUser;

public sealed record CurrentUserContext(
    bool IsAuthenticated,
    string? ExternalId,
    Guid? UserId,
    Guid? ActiveUserRoleId,
    Guid? ActiveRoleId,
    Guid? ActiveCenterId)
    : ICurrentUserContext;