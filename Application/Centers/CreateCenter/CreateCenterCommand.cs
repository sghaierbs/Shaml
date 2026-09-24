namespace Application.Centers.CreateCenter;

public sealed record CreateCenterCommand(
    string Code,
    string Name,
    string Region,
    string City,
    string? Address = null,
    string? Phone = null,
    string? Email = null);