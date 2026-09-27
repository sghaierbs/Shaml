using Domain.Cases;

namespace Application.Common.Interfaces;

public interface ICaseRepository
{
    Task AddAsync(
        Case shamlCase,
        CancellationToken cancellationToken = default);
    
    Task<Case?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}