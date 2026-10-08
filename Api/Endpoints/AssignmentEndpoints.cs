using Api.Authentication;
using Application.Assignments;
using Application.Assignments.ClaimAssignment;
using Application.Assignments.CompleteAssignment;
using Application.Assignments.CreateAssignment;
using Application.Assignments.GetMyWork;
using Application.Assignments.GetRoleQueue;
using Application.Assignments.StartAssignment;
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

        // --------------------------------------------------
        // Claim assignment
        // --------------------------------------------------

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
            .RequireAuthorization(AuthorizationPolicies.ShamlUser);

        // --------------------------------------------------
        // Start assignment
        // --------------------------------------------------

        app.MapPost(
                "/api/assignments/{assignmentId:guid}/start",
                async (
                    Guid assignmentId,
                    StartAssignmentHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.HandleAsync(
                        new StartAssignmentCommand(assignmentId),
                        cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization(AuthorizationPolicies.ShamlUser);
        
        app.MapPost(
                "/api/assignments/{assignmentId:guid}/complete",
                async (
                    Guid assignmentId,
                    CompleteAssignmentHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.HandleAsync(
                        new CompleteAssignmentCommand(
                            assignmentId),
                        cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization(
                AuthorizationPolicies.ShamlUser);

        // --------------------------------------------------
        // Role queue
        // --------------------------------------------------

        app.MapGet(
                "/api/assignments/queue",
                async (
                    GetRoleQueueHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization(AuthorizationPolicies.ShamlUser);

        // --------------------------------------------------
        // My work
        // --------------------------------------------------

        app.MapGet(
                "/api/assignments/mine",
                async (
                    GetMyWorkHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization(AuthorizationPolicies.ShamlUser);

        return app;
    }
}

public sealed record CreateRoleQueueAssignmentRequest(
    Guid CaseId,
    Guid RoleId,
    Guid CenterId);