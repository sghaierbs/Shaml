using Application.Identity.CurrentUser;
using Application.Identity.GetUserRoles;

namespace Api.Endpoints;

public static class CurrentUserEndpoints
{
    public static IEndpointRouteBuilder MapCurrentUserEndpoints(
        this IEndpointRouteBuilder app)
    {
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

        // Temporary ExternalId header until IAM authentication is integrated.
        app.MapGet(
            "/api/me/roles",
            async (
                HttpRequest request,
                GetUserRolesHandler handler,
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

                var roles = await handler.HandleAsync(
                    new GetUserRolesQuery(externalId),
                    cancellationToken);

                return Results.Ok(roles);
            });

        return app;
    }
}