using Domain.Common;

namespace Domain.Cases.Events;

public sealed record CaseCompletedEvent(
    Guid CaseId,
    DateTime OccurredOnUtc
) : IDomainEvent;