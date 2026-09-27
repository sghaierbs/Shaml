namespace Api.Contracts.Cases;

public sealed record RouteToSpecialistRequest(
    Guid SpecialistRoleId,
    Guid CenterId);