using Application.Common.Interfaces;

namespace Application.Centers.GetCenters;

public sealed class GetCentersHandler
{
    private readonly ICenterRepository _centerRepository;

    public GetCentersHandler(ICenterRepository centerRepository)
    {
        _centerRepository = centerRepository;
    }

    public async Task<GetCentersResult> HandleAsync(
        GetCentersQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (centers, totalCount) =
            await _centerRepository.GetPagedAsync(
                page,
                pageSize,
                query.Search,
                query.Region,
                query.City,
                query.Status,
                cancellationToken);

        var items = centers
            .Select(center => new CenterListItem(
                center.Id,
                center.Code,
                center.Name,
                center.Region,
                center.City,
                center.Address,
                center.Phone,
                center.Email,
                center.Status.ToString(),
                center.CreatedAtUtc))
            .ToList();

        return new GetCentersResult(
            items,
            page,
            pageSize,
            totalCount);
    }
}