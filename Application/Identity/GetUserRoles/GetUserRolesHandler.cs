using Application.Common.Interfaces;

namespace Application.Identity.GetUserRoles;

public sealed class GetUserRolesHandler
{
    private readonly IUserRepository _users;
    private readonly IUserRoleRepository _userRoles;
    private readonly IRoleRepository _roles;
    private readonly ICenterRepository _centers;

    public GetUserRolesHandler(
        IUserRepository users,
        IUserRoleRepository userRoles,
        IRoleRepository roles,
        ICenterRepository centers)
    {
        _users = users;
        _userRoles = userRoles;
        _roles = roles;
        _centers = centers;
    }

    public async Task<IReadOnlyCollection<UserRoleResult>> HandleAsync(
        GetUserRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByExternalIdAsync(
            query.ExternalId,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Authenticated user does not exist in Shaml.");
        }

        var assignments = await _userRoles.GetByUserIdAsync(
            user.Id,
            cancellationToken);

        var results = new List<UserRoleResult>();

        foreach (var assignment in assignments)
        {
            var role = await _roles.GetByIdAsync(
                assignment.RoleId,
                cancellationToken);

            if (role is null || !role.IsActive)
            {
                continue;
            }

            string? centerName = null;

            if (assignment.CenterId.HasValue)
            {
                var center = await _centers.GetByIdAsync(
                    assignment.CenterId.Value,
                    cancellationToken);

                centerName = center?.Name;
            }

            results.Add(new UserRoleResult(
                UserRoleId: assignment.Id,
                RoleId: role.Id,
                RoleCode: role.Code,
                RoleName: role.Name,
                Portal: role.Portal,
                ScopeType: role.ScopeType,
                CenterId: assignment.CenterId,
                CenterName: centerName,
                IsDefault: assignment.IsDefault));
        }

        return results;
    }
}