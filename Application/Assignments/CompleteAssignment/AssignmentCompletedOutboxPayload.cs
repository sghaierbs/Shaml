namespace Application.Assignments.CompleteAssignment;

public sealed record AssignmentCompletedOutboxPayload(
    Guid AssignmentId,
    Guid CaseId,
    string TaskCode,
    DateTime CompletedAtUtc);