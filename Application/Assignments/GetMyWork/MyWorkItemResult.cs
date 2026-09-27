using Domain.Assignments;

namespace Application.Assignments.GetMyWork;

public sealed record MyWorkItemResult(
    Guid AssignmentId,
    Guid CaseId,
    string TaskCode,
    AssignmentStatus Status,
    AssignmentTargetType TargetType,
    DateTime CreatedAtUtc);