using Domain.Identity;

namespace Application.Common.Interfaces;

public interface IUserRoleRepository
{
    Task AddAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid userId,
        Guid roleId,
        Guid? centerId,
        CancellationToken cancellationToken = default);
    
    Task<UserRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserRole>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}