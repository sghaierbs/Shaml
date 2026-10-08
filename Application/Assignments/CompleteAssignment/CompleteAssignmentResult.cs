using Domain.Assignments;

namespace Application.Assignments.CompleteAssignment;

public sealed record CompleteAssignmentResult(
    Guid AssignmentId,
    AssignmentStatus Status,
    DateTime CompletedAtUtc);