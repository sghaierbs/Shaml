using Application.Common.Interfaces;
using Domain.Cases;

namespace Infrastructure.Persistence.Repositories;

public sealed class CaseRepository : ICaseRepository
{
    private readonly ShamlDbContext _dbContext;

    public CaseRepository(ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Case shamlCase,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Cases.AddAsync(
            shamlCase,
            cancellationToken);
    }
}