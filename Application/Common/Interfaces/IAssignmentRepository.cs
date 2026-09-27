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
}