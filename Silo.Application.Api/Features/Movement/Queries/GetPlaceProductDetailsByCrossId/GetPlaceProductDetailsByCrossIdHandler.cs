namespace Silo.Application.Api.Features;
public class GetPlaceProductDetailsByCrossIdHandler(WmsApiContext apiContext) : IRequestHandler<GetPlaceProductDetailsByCrossIdQuery, GetPlaceProductDetailsByCrossIdVm>
{
    public async Task<GetPlaceProductDetailsByCrossIdVm> Handle(GetPlaceProductDetailsByCrossIdQuery request, CancellationToken cancellationToken)
    {
        var result = await apiContext.TagsMovements
            .Include(p => p.MovementAction)
            .Where(p => p.MovementAction.MovementActionTruckCrossId == request.TruckCrossId
                        && p.ProductCode == request.ProductCode
                        && p.RMovementActionDocumentId == request.DocumentCode)
            .Select(p => new GetPlaceProductDetailsByCrossIdDto()
            {
                ProductCode = p.ProductCode,
                ProductSerial = p.ProductSerial,
                ProductCount = p.ProductCount.Value
            })
            .ToListAsync();

        return new()
        {
            List = result
        };
    }
}
