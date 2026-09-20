using Domain.Common;

namespace Shaml.Domain.Cases.Events;

public sealed record CaseStartedEvent(
    Guid CaseId,
    DateTime OccurredOnUtc
) : IDomainEvent;