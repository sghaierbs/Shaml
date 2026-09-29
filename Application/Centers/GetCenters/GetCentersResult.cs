namespace Application.Centers.GetCenters;

public sealed record GetCentersResult(
    IReadOnlyList<CenterListItem> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages =>
        TotalCount == 0
            ? 0
            : (int)Math.Ceiling((double)TotalCount / PageSize);
}