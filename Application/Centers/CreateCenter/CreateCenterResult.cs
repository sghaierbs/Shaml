namespace Application.Centers.CreateCenter;

public sealed record CreateCenterResult(
    Guid CenterId,
    string Code,
    string Name,
    string Region,
    string City);