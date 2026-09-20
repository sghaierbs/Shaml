using Microsoft.Extensions.Logging;
using Application.Common.Events;
using Domain.Cases.Events;

namespace Application.Cases.EventHandlers;

public sealed class CaseCreatedEventHandler
    : IDomainEventHandler<CaseCreatedEvent>
{
    private readonly ILogger<CaseCreatedEventHandler> _logger;

    public CaseCreatedEventHandler(
        ILogger<CaseCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(
        CaseCreatedEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Case created. CaseId: {CaseId}, OccurredAt: {OccurredAt}",
            domainEvent.CaseId,
            domainEvent.OccurredOnUtc);

        return Task.CompletedTask;
    }
}