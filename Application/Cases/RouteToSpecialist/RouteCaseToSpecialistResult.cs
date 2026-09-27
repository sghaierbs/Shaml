using Domain.Cases;

namespace Application.Cases.RouteToSpecialist;

public sealed record RouteCaseToSpecialistResult(
    Guid CaseId,
    CaseStatus CaseStatus,
    Guid AssignmentId);