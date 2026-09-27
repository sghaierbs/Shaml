using Application.Assignments;
using Application.Assignments.ClaimAssignment;
using Application.Assignments.CreateAssignment;
using Domain.Assignments;

namespace Api.Endpoints;

public static class AssignmentEndpoints
{
    public static IEndpointRouteBuilder MapAssignmentEndpoints(
        this IEndpointRouteBuilder app)
    {
        if (app.ServiceProvider
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment())
        {
            app.MapPost(
                "/api/dev/assignments/role-queue",
                async (
                    CreateRoleQueueAssignmentRequest request,
                    CreateAssignmentHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.HandleAsync(
                        new CreateAssignmentCommand(
                            request.CaseId,
                            AssignmentTaskCodes.ReviewCase,
                            AssignmentTargetType.RoleQueue,
                            TargetRoleId: request.RoleId,
                            TargetCenterId: request.CenterId),
                        cancellationToken);

                    return Results.Ok(result);
                });
        }
        
        app.MapPost(
                "/api/assignments/{assignmentId:guid}/claim",
                async (
                    Guid assignmentId,
                    ClaimAssignmentHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.HandleAsync(
                        new ClaimAssignmentCommand(assignmentId),
                        cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization();

        return app;
    }
}

public sealed record CreateRoleQueueAssignmentRequest(
    Guid CaseId,
    Guid RoleId,
    Guid CenterId);