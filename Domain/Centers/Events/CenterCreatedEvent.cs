using Domain.Common;

namespace Domain.Centers.Events;

public sealed record CenterCreatedEvent(Guid CenterId, string CenterCode) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}