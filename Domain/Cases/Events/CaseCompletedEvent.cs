using Domain.Common;

namespace Shaml.Domain.Cases.Events;

public sealed record CaseCompletedEvent(
    Guid CaseId,
    DateTime OccurredOnUtc
) : IDomainEvent;