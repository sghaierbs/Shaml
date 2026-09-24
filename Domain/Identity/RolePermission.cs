using Domain.Common;

namespace Domain.Identity;

public class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    private RolePermission()
    {
    }

    private RolePermission(
        Guid id,
        Guid roleId,
        Guid permissionId) : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static RolePermission Create(
        Guid roleId,
        Guid permissionId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException(
                "Role is required.",
                nameof(roleId));

        if (permissionId == Guid.Empty)
            throw new ArgumentException(
                "Permission is required.",
                nameof(permissionId));

        return new RolePermission(
            Guid.NewGuid(),
            roleId,
            permissionId);
    }
}