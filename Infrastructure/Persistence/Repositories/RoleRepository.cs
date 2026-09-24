using Application.Common.Interfaces;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ShamlDbContext _dbContext;

    public RoleRepository(ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Role?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<Role?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }
}