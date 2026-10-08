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

    public Task<int> CountActiveByCenterAsync(Guid centerId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Cases.CountAsync(
            x => x.CenterId == centerId &&
                 (x.Status == CaseStatus.New ||
                  x.Status == CaseStatus.WaitingForSpecialist ||
                  x.Status == CaseStatus.InProgress ||
                  x.Status == CaseStatus.UnderStudy ||
                  x.Status == CaseStatus.PendingClassificationApproval),
            cancellationToken);
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