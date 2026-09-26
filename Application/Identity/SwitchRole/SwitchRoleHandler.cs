using Application.Identity.CurrentUser;
using Application.Identity.Tokens;

namespace Application.Identity.SwitchRole;

public sealed class SwitchRoleHandler
{
    private readonly ICurrentUserContextResolver _currentUserContextResolver;
    private readonly ITokenService _tokenService;

    public SwitchRoleHandler(
        ICurrentUserContextResolver currentUserContextResolver,
        ITokenService tokenService)
    {
        _currentUserContextResolver = currentUserContextResolver;
        _tokenService = tokenService;
    }

    public async Task<SwitchRoleResult> HandleAsync(
        SwitchRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var context =
            await _currentUserContextResolver.ResolveAsync(
                command.ExternalId,
                command.UserRoleId,
                cancellationToken);

        // Next: resolve permissions for context.RoleId.

        throw new NotImplementedException();
    }
}