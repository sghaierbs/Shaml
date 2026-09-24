using Application.Identity.AssignRole;
using Application.Identity.ProvisionUser;

namespace Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users");

        group.MapPost("/", ProvisionUser);
        group.MapPost("/{userId:guid}/roles", AssignRole);

        return endpoints;
    }

    private static async Task<IResult> ProvisionUser(
        ProvisionUserRequest request,
        ProvisionUserHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ProvisionUserCommand(
                request.ExternalId,
                request.FullName,
                request.Email),
            cancellationToken);

        return Results.Created(
            $"/api/users/{result.UserId}",
            result);
    }

    private static async Task<IResult> AssignRole(
        Guid userId,
        AssignRoleRequest request,
        AssignRoleToUserHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AssignRoleToUserCommand(
                userId,
                request.RoleId,
                request.CenterId,
                request.IsDefault),
            cancellationToken);

        return Results.Created(
            $"/api/users/{userId}/roles/{result.UserRoleId}",
            result);
    }

    private sealed record ProvisionUserRequest(
        string ExternalId,
        string FullName,
        string? Email);

    private sealed record AssignRoleRequest(
        Guid RoleId,
        Guid? CenterId,
        bool IsDefault = false);
}