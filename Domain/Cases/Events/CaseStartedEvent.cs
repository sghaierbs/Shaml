using Domain.Common;

namespace Domain.Cases.Events;

public sealed record CaseStartedEvent(
    Guid CaseId,
    DateTime OccurredOnUtc
) : IDomainEvent;