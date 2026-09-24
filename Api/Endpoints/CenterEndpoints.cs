using Application.Centers.CreateCenter;

namespace Api.Endpoints;

public static class CenterEndpoints
{
    public static IEndpointRouteBuilder MapCenterEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/centers");

        group.MapPost("/", CreateCenter);

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

    private sealed record CreateCenterRequest(
        string Code,
        string Name,
        string Region,
        string City,
        string? Address,
        string? Phone,
        string? Email);
}