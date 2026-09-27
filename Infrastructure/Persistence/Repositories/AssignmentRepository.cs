using Application.Common.Interfaces;
using Domain.Assignments;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class AssignmentRepository : IAssignmentRepository
{
    private readonly ShamlDbContext _dbContext;

    public AssignmentRepository(
        ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Assignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Assignments
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Assignment assignment,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Assignments.AddAsync(
            assignment,
            cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Assignment>> GetOpenRoleQueueAsync(
        Guid roleId,
        Guid centerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments
            .AsNoTracking()
            .Where(x =>
                x.TargetType == AssignmentTargetType.RoleQueue &&
                x.TargetRoleId == roleId &&
                x.TargetCenterId == centerId &&
                x.Status == AssignmentStatus.Open)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Assignment>> GetWorkForUserRoleAsync(
        Guid userRoleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments
            .AsNoTracking()
            .Where(x =>
                x.Status != AssignmentStatus.Completed &&
                x.Status != AssignmentStatus.Cancelled &&
                (
                    x.TargetUserRoleId == userRoleId ||
                    x.ClaimedByUserRoleId == userRoleId
                ))
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<Assignment>> GetWorkForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments
            .AsNoTracking()
            .Where(x =>
                x.TargetType == AssignmentTargetType.User &&
                x.TargetUserId == userId &&
                x.Status != AssignmentStatus.Completed &&
                x.Status != AssignmentStatus.Cancelled)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}