namespace Application.Identity.AssignRole;

public sealed record AssignRoleToUserCommand(
    Guid UserId,
    Guid RoleId,
    Guid? CenterId = null,
    bool IsDefault = false
    );