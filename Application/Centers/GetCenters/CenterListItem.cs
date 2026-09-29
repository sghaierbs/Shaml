namespace Application.Centers.GetCenters;

public sealed record CenterListItem(
    Guid Id,
    string Code,
    string Name,
    string Region,
    string City,
    string? Address,
    string? Phone,
    string? Email,
    string Status,
    DateTime CreatedAtUtc);