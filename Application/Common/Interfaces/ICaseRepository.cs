using Domain.Cases;

namespace Application.Common.Interfaces;

public interface ICaseRepository
{
    /// <summary>Counts admitted active cases only; excludes pending capacity queue and terminal cases.</summary>
    Task<int> CountActiveByCenterAsync(Guid centerId, CancellationToken cancellationToken = default);

    Task AddAsync(
        Case shamlCase,
        CancellationToken cancellationToken = default);
    
    Task<Case?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}