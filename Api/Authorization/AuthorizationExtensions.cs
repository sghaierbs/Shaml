namespace Api.Authorization;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permission)
    {
        return builder.RequireAuthorization(policy => policy.RequireClaim("permission", permission));
    }
}