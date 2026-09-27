using Application.Identity.CurrentUser;
using Application.Identity.Permissions;
using Application.Identity.Tokens;

namespace Application.Identity.IssueUserToken;

public sealed class IssueUserTokenHandler
{
    private readonly ICurrentUserContextResolver _currentUserContextResolver;
    private readonly IPermissionService _permissionService;
    private readonly ITokenService _tokenService;

    public IssueUserTokenHandler(
        ICurrentUserContextResolver currentUserContextResolver,
        IPermissionService permissionService,
        ITokenService tokenService)
    {
        _currentUserContextResolver = currentUserContextResolver;
        _permissionService = permissionService;
        _tokenService = tokenService;
    }

    public async Task<IssueUserTokenResult> HandleAsync(
        IssueUserTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        // null means: resolve the user's default UserRole.
        var context =
            await _currentUserContextResolver.ResolveAsync(
                command.ExternalId,
                requestedUserRoleId: null,
                cancellationToken);

        var permissions =
            await _permissionService.GetPermissionsAsync(
                context.RoleId,
                cancellationToken);

        var token = _tokenService.IssueToken(
            context,
            permissions);

        return new IssueUserTokenResult(
            token.AccessToken,
            token.ExpiresAtUtc);
    }
}