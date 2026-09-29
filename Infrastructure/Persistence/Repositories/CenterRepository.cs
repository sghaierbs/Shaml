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
    
    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Centers
            .AnyAsync(
                center => center.Code == code,
                cancellationToken);
    }
    
    public async Task<(IReadOnlyList<Center> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? region,
        string? city,
        CenterStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Centers
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(c =>
                EF.Functions.ILike(c.Code, $"%{normalizedSearch}%") ||
                EF.Functions.ILike(c.Name, $"%{normalizedSearch}%") ||
                EF.Functions.ILike(c.City, $"%{normalizedSearch}%"));
        }

        if (!string.IsNullOrWhiteSpace(region))
        {
            query = query.Where(c => c.Region == region);
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(c => c.City == city);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}