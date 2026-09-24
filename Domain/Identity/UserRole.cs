using Domain.Common;

namespace Domain.Identity;

public class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public Guid? CenterId { get; private set; }

    public bool IsDefault { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private UserRole()
    {
    }

    private UserRole(
        Guid id,
        Guid userId,
        Guid roleId,
        Guid? centerId,
        bool isDefault) : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        CenterId = centerId;
        IsDefault = isDefault;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static UserRole Create(
        Guid userId,
        Role role,
        Guid? centerId = null,
        bool isDefault = false)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User is required.",
                nameof(userId));

        ArgumentNullException.ThrowIfNull(role);

        if (role.ScopeType == ScopeType.Center &&
            (!centerId.HasValue || centerId.Value == Guid.Empty))
        {
            throw new ArgumentException(
                "Center is required for a center-scoped role.",
                nameof(centerId));
        }

        if (role.ScopeType == ScopeType.Organization &&
            centerId.HasValue)
        {
            throw new ArgumentException(
                "Organization-scoped roles cannot have a center.",
                nameof(centerId));
        }

        return new UserRole(
            Guid.NewGuid(),
            userId,
            role.Id,
            centerId,
            isDefault);
    }
}