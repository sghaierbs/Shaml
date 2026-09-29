using Api.Authorization;
using Application.Centers.CreateCenter;
using Application.Identity.Permissions;
using Application.Centers.GetCenters;
using Domain.Centers;

namespace Api.Endpoints;

public static class CenterEndpoints
{
    public static IEndpointRouteBuilder MapCenterEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/centers");
        
        group.MapGet("/", GetCenters)
            .RequirePermission(PermissionCodes.CenterView);

        group.MapPost("/", CreateCenter)
            .RequirePermission(PermissionCodes.CenterManage);

        return endpoints;
    }

    
    
    private static async Task<IResult> CreateCenter(
        CreateCenterRequest request,
        CreateCenterHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateCenterCommand(
                request.Code,
                request.Name,
                request.Region,
                request.City,
                request.Address,
                request.Phone,
                request.Email),
            cancellationToken);

        return Results.Created(
            $"/api/centers/{result.CenterId}",
            result);
    }
    
    private static async Task<IResult> GetCenters(
        int page,
        int pageSize,
        string? search,
        string? region,
        string? city,
        CenterStatus? status,
        GetCentersHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetCentersQuery(
                page <= 0 ? 1 : page,
                pageSize <= 0 ? 10 : pageSize,
                search,
                region,
                city,
                status),
            cancellationToken);

        return Results.Ok(result);
    }

    private sealed record CreateCenterRequest(
        string Code,
        string Name,
        string Region,
        string City,
        string? Address,
        string? Phone,
        string? Email);
}