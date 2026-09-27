using Domain.Assignments;

namespace Application.Assignments.ClaimAssignment;

public sealed record ClaimAssignmentResult(
    Guid AssignmentId,
    AssignmentStatus Status,
    Guid ClaimedByUserRoleId,
    DateTime ClaimedAtUtc);