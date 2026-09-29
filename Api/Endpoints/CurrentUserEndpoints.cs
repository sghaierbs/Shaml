using Api.Authorization;
using Application.Common.Interfaces;
using Application.Identity.CurrentUser;
using Application.Identity.GetUserRoles;
using Application.Identity.IssueUserToken;
using Application.Identity.Permissions;
using Application.Identity.SwitchRole;

namespace Api.Endpoints;

public static class CurrentUserEndpoints
{
    public static IEndpointRouteBuilder MapCurrentUserEndpoints(this IEndpointRouteBuilder app)
    {
        
        app.MapGet(
                "/api/dev/protected-case-view",
                (ICurrentUserContext currentUser) =>
                {
                    return Results.Ok(new
                    {
                        message = "You have case.view permission.",
                        currentUser.UserId,
                        currentUser.ActiveUserRoleId,
                        currentUser.RoleId,
                        currentUser.Portal,
                        currentUser.ScopeType,
                        currentUser.CenterId
                    });
                })
            .RequirePermission(PermissionCodes.CaseView);
        
        
        app.MapPost(
            "/api/dev/token",
            async (
                IssueUserTokenRequest request,
                IssueUserTokenHandler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(
                    new IssueUserTokenCommand(request.ExternalId),
                    cancellationToken);

                return Results.Ok(result);
            });
    
        
        // Temporary development endpoint.
        app.MapGet(
                "/api/me",
                (ICurrentUserContext currentUser) =>
                {
                    return Results.Ok(new
                    {
                        currentUser.UserId,
                        currentUser.ExternalId,
                        currentUser.ActiveUserRoleId,
                        currentUser.RoleId,
                        currentUser.Portal,
                        currentUser.ScopeType,
                        currentUser.CenterId
                    });
                })
            .RequireAuthorization();
        
        app.MapPost(
                "/api/me/switch-role",
                async (
                    SwitchRoleRequest request,
                    SwitchRoleHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.HandleAsync(
                        new SwitchRoleCommand(request.UserRoleId),
                        cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization();

        // Temporary ExternalId header until IAM authentication is integrated.
        app.MapGet(
                "/api/me/roles",
                async (GetUserRolesHandler handler, CancellationToken cancellationToken) =>
                {
                    var roles = await handler.HandleAsync(new GetUserRolesQuery(), cancellationToken);
                    return Results.Ok(roles);
                })
            .RequireAuthorization();

        return app;
    }
}

public sealed record SwitchRoleRequest(Guid UserRoleId);
public sealed record IssueUserTokenRequest(string ExternalId);