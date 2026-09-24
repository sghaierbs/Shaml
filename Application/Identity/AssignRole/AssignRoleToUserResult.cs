namespace Application.Identity.AssignRole;

public sealed record AssignRoleToUserResult(
    Guid UserRoleId,
    Guid UserId,
    Guid RoleId,
    Guid? CenterId);