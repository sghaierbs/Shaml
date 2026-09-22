using Application.Common.Interfaces;
using Domain.Centers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CenterRepository : ICenterRepository
{
    private readonly ShamlDbContext _dbContext;

    public CenterRepository(ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Center center,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Centers.AddAsync(
            center,
            cancellationToken);
    }

    public Task<Center?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Centers
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}