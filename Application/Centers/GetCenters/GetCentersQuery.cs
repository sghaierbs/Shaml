using Domain.Centers;

namespace Application.Centers.GetCenters;

public sealed record GetCentersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? Region = null,
    string? City = null,
    CenterStatus? Status = null);