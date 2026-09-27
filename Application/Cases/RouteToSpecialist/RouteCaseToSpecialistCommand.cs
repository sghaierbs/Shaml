namespace Application.Cases.RouteToSpecialist;

public sealed record RouteCaseToSpecialistCommand(
    Guid CaseId,
    Guid SpecialistRoleId,
    Guid CenterId);