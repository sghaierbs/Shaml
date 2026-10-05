using Domain.Assignments;

namespace Application.Assignments.StartAssignment;

public sealed record StartAssignmentResult(
    Guid AssignmentId,
    AssignmentStatus Status,
    DateTime StartedAtUtc);