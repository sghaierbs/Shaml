using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Identity.CurrentUser;

public sealed class CurrentUserContextResolver : ICurrentUserContextResolver
{
    private readonly IUserRepository _users;
    private readonly IUserRoleRepository _userRoles;
    private readonly IRoleRepository _roles;

    public CurrentUserContextResolver(
        IUserRepository users,
        IUserRoleRepository userRoles,
        IRoleRepository roles)
    {
        _users = users;
        _userRoles = userRoles;
        _roles = roles;
    }

    public async Task<ICurrentUserContext> ResolveAsync(
        string externalId,
        Guid? requestedUserRoleId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new UnauthorizedAccessException(
                "Authenticated external user identifier is missing.");
        }

        var user = await _users.GetByExternalIdAsync(
            externalId,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                $"No Shaml user exists for external identity '{externalId}'.");
        }

        var userRole = requestedUserRoleId.HasValue
            ? await ResolveRequestedUserRoleAsync(
                user.Id,
                requestedUserRoleId.Value,
                cancellationToken)
            : await ResolveDefaultUserRoleAsync(
                user.Id,
                cancellationToken);

        var role = await _roles.GetByIdAsync(
            userRole.RoleId,
            cancellationToken);

        if (role is null)
        {
            throw new UnauthorizedAccessException(
                "The selected role no longer exists.");
        }

        if (!role.IsActive)
        {
            throw new UnauthorizedAccessException(
                "The selected role is inactive.");
        }

        return new CurrentUserContext(
            UserId: user.Id,
            ExternalId: user.ExternalId,
            ActiveUserRoleId: userRole.Id,
            RoleId: role.Id,
            Portal: role.Portal,
            ScopeType: role.ScopeType,
            CenterId: userRole.CenterId);
    }

    private async Task<Domain.Identity.UserRole> ResolveRequestedUserRoleAsync(
        Guid userId,
        Guid requestedUserRoleId,
        CancellationToken cancellationToken)
    {
        var userRole = await _userRoles.GetByIdAsync(
            requestedUserRoleId,
            cancellationToken);

        if (userRole is null)
        {
            throw new UnauthorizedAccessException(
                "The requested role assignment does not exist.");
        }

        if (userRole.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "The requested role assignment does not belong to the authenticated user.");
        }

        return userRole;
    }

    private async Task<Domain.Identity.UserRole> ResolveDefaultUserRoleAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var userRoles = await _userRoles.GetByUserIdAsync(
            userId,
            cancellationToken);

        var defaultUserRole = userRoles.FirstOrDefault(
            x => x.IsDefault);

        if (defaultUserRole is null)
        {
            throw new UnauthorizedAccessException(
                "The authenticated user has no default role assignment.");
        }

        return defaultUserRole;
    }
}