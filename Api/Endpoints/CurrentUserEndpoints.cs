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
        
        if (app.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
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
        }
        
        // Temporary development endpoint.
        app.MapGet(
            "/api/dev/current-user",
            async (
                HttpRequest request,
                ICurrentUserContextResolver resolver,
                CancellationToken cancellationToken) =>
            {
                var externalId =
                    request.Headers["X-External-Id"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(externalId))
                {
                    return Results.BadRequest(new
                    {
                        error = "X-External-Id header is required."
                    });
                }

                Guid? requestedUserRoleId = null;

                var userRoleHeader =
                    request.Headers["X-User-Role-Id"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(userRoleHeader))
                {
                    if (!Guid.TryParse(
                            userRoleHeader,
                            out var parsedUserRoleId))
                    {
                        return Results.BadRequest(new
                        {
                            error = "X-User-Role-Id must be a valid GUID."
                        });
                    }

                    requestedUserRoleId = parsedUserRoleId;
                }

                var context = await resolver.ResolveAsync(
                    externalId,
                    requestedUserRoleId,
                    cancellationToken);

                return Results.Ok(context);
            });
        
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