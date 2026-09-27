using Domain.Common.Outbox;

namespace Application.Common.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);
}