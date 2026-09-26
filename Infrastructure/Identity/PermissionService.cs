using Application.Identity.Permissions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public sealed class PermissionService : IPermissionService
{
    private readonly ShamlDbContext _dbContext;

    public PermissionService(ShamlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Join(
                _dbContext.Permissions.AsNoTracking(),
                rp => rp.PermissionId,
                permission => permission.Id,
                (rp, permission) => permission.Code)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPermissionAsync(
        Guid roleId,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            return false;

        var normalizedCode = permissionCode.Trim().ToLowerInvariant();

        return await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Join(
                _dbContext.Permissions.AsNoTracking(),
                rp => rp.PermissionId,
                permission => permission.Id,
                (rp, permission) => permission.Code)
            .AnyAsync(
                code => code == normalizedCode,
                cancellationToken);
    }
}