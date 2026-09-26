using Application.Common.Interfaces;

namespace Application.Identity.CurrentUser;

public interface ICurrentUserContextResolver
{
    Task<ICurrentUserContext> ResolveAsync(
        string externalId,
        Guid? requestedUserRoleId = null,
        CancellationToken cancellationToken = default);
}