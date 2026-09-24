using Domain.Identity;

namespace Application.Common.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Role?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}