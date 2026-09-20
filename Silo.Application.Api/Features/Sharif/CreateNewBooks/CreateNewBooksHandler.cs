using DocumentFormat.OpenXml.InkML;

namespace Silo.Application.Api.Features;

public class CreateNewBooksHandler
    (IWmsBusiness wmsBusiness, WmsApiContext wmsApiContext) : IRequestHandler<CreateNewBooksCommand, CreateNewBooksVm>
{
   
    public async Task<CreateNewBooksVm> Handle(
        CreateNewBooksCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Data == null || request.Data.Count == 0)
        {
            return new CreateNewBooksVm
            {
                Result = false
            };
        }

        var products = request.Data
           .Where(x => !string.IsNullOrWhiteSpace(x.PID))
           .GroupBy(x => x.PID)
           .Select(g => g.First())
           .Select(x => new SaveProductCommand
           {
               ProductCode = x.PID!,
               ProductTitle = x.Title ?? "",
               ProductENTitle = string.Empty,
               ProductPackValue = 0,
               ProductPackWeight = 0,
               ProductPackVolume = 0m,
               ProductCountInPack = 0,
               ProductValue = 0m,
               ProductTechnicalCode = string.Empty,
               ProductProperties = string.Empty,
               ProductType = "Book",
               ProductStatus = "Active",
               ProductSize = string.Empty,
               ProductUnit = string.Empty,
               ProductGalleryId = 0,
               ProductGroup = "Books",
               ProductBrand = string.Empty,
               ProductSubGroup = string.Empty,
               ProductClass = string.Empty,
               IsActive = true
           })
            .ToList();

        int result = wmsBusiness.SSaveProductBatch(products);

        wmsApiContext.Prints.AddRange(
        request.Data
           .Where(x => !string.IsNullOrWhiteSpace(x.PID))
           .GroupBy(x => x.PID)
           .Select(g => g.First())
           .Select(x => new Print()
           {
               ProductSerial = x.Barcode,
               ProductCode = x.PID,
               ProductName = x.Title
           }));

       await wmsApiContext.SaveChangesAsync();

        return new CreateNewBooksVm
        {
            Result = result == 1
        };
    }
}
