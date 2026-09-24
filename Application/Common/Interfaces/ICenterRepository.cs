using Domain.Centers;

namespace Application.Common.Interfaces;

public interface ICenterRepository
{
    Task AddAsync(
        Center center,
        CancellationToken cancellationToken = default);

    Task<Center?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}