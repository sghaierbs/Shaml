using Domain.Common;

namespace Domain.Cases.Events;

public sealed record CaseCreatedEvent(Guid CaseId, DateTime OccurredOnUtc ) : IDomainEvent;