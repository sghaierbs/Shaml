using Application.Common.Interfaces;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly ShamlDbContext _dbContext;

    public UserRoleRepository(ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.UserRoles.AddAsync(
            userRole,
            cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Guid userId,
        Guid roleId,
        Guid? centerId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRoles.AnyAsync(
            x =>
                x.UserId == userId &&
                x.RoleId == roleId &&
                x.CenterId == centerId,
            cancellationToken);
    }
    
    public Task<UserRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserRole>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}