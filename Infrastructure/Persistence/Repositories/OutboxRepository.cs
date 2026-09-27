using Application.Common.Interfaces;
using Domain.Common.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly ShamlDbContext _dbContext;

    public OutboxRepository(ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.OutboxMessages.AddAsync(
            message,
            cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}