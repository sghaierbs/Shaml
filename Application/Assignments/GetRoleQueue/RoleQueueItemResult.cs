using Domain.Assignments;

namespace Application.Assignments.GetRoleQueue;

public sealed record RoleQueueItemResult(
    Guid AssignmentId,
    Guid CaseId,
    string TaskCode,
    AssignmentStatus Status,
    DateTime CreatedAtUtc);