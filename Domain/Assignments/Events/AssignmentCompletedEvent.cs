using Domain.Common;

namespace Domain.Assignments.Events;

public sealed record AssignmentCompletedEvent(
    Guid AssignmentId,
    Guid CaseId,
    string TaskCode,
    DateTime CompletedAtUtc
) : IDomainEvent
{
    public DateTime OccurredOnUtc => CompletedAtUtc;
}