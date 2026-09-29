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
    
    Task<(IReadOnlyList<Center> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? region,
        string? city,
        CenterStatus? status,
        CancellationToken cancellationToken = default);
}