namespace Silo.Application.Api.Features;
public class GetLoadedCargoProductsHandler(WmsApiContext apiContext) : IRequestHandler<GetLoadedCargoProductsQuery, GetLoadedCargoProductsVm>
{
    public async Task<GetLoadedCargoProductsVm> Handle(GetLoadedCargoProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await apiContext.TagsMovements
          .Include(p => p.MovementAction)
          .Include(p=> p.Product)
          .Where(p => p.MovementAction.MovementActionTruckCrossId == request.TruckCrossId)
          .Select(p => new GetLoadedCargoProductsDto()
          {
              ProductCode = p.ProductCode,
              ProductSerial = p.ProductSerial,
              ProductCount = p.ProductCount.Value,
              DocumentCode = p.RMovementActionDocumentId,
              ProductName = p.Product.Title
          })
          .ToListAsync();

        return new()
        {
            LoadedPoducts = result
        };
    }
}
