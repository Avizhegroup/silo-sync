namespace Silo.Application.Api.Features;
public class GetPlaceByTruckCrossIdHandler(WmsApiContext apiContext) : IRequestHandler<GetPlaceByTruckCrossIdQuery, GetPlaceByTruckCrossIdVm>
{
    public async Task<GetPlaceByTruckCrossIdVm> Handle(GetPlaceByTruckCrossIdQuery request, CancellationToken cancellationToken)
    {
        var actions = apiContext.MovementActions
            .Where(p => p.MovementActionTruckCrossId == request.TruckCrossId);

        if (actions.Any())
        {
            var tagsMovements = await apiContext.TagsMovements
                .Include(p=>p.Tag)
                .Where(p => actions.Select(p=>p.MovementActionId).Contains(p.RMovementActionId.Value))
                .ToListAsync(cancellationToken);

            var aggregatedData = tagsMovements
               .GroupBy(tm =>  new { tm.ProductCode, tm.RMovementActionDocumentId })
               .Select(group => new GetPlaceByTruckCrossIdDto
               {
                   DocumentCode = group.Key.RMovementActionDocumentId,
                   ProductCode = group.Key.ProductCode,
                   ProductName = group.FirstOrDefault()?.Tag.ProductName ?? "",
                   ProductCount = group.Count(),
                   SumCount = group.Sum(tm => tm.ProductCount ?? 0)
               })
               .ToList();

            return new GetPlaceByTruckCrossIdVm
            {
                ActionIds = actions.Select(p => p.MovementActionId).ToList(),
                PlaceProducts = aggregatedData
            };
        }

        return new GetPlaceByTruckCrossIdVm
        {
            ActionIds = null,
            PlaceProducts = null
        };
    }
}
