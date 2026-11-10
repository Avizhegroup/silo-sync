using Silo.Application.Shared.Features;

namespace Silo.Application.Api.Features;

public class GetDashboardItemsHandler(WmsApiContext context)
    : IRequestHandler<GetDashboardItemsQuery, List<GetDashboardItemsVm>>
{
    public async Task<List<GetDashboardItemsVm>> Handle(
        GetDashboardItemsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Set<DashboardItems>()
            .AsNoTracking()
            .Select(item => new GetDashboardItemsVm
            {
                ItemCode = item.ItemCode,
                ItemTitle = item.ItemTitle,
                ItemType = item.ItemType
            })
            .ToListAsync(cancellationToken);
    }
}
