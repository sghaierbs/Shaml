using Domain.Assignments;

namespace Application.Common.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Assignment assignment,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Assignment>> GetOpenRoleQueueAsync(
        Guid roleId,
        Guid centerId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Assignment>> GetWorkForUserRoleAsync(
        Guid userRoleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Assignment>> GetWorkForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}