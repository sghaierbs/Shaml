using Application.Common.Interfaces;
using Domain.Cases;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task<Case?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cases
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}