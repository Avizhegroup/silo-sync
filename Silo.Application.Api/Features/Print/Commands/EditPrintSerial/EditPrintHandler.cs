namespace Silo.Application.Features;

public class EditPrintHandler(WmsApiContext context) : IRequestHandler<EditPrintCommand, EditPrintVm>
{
    public async Task<EditPrintVm> Handle(EditPrintCommand request, CancellationToken cancellationToken)
    {
        var print = await context.Prints.FirstOrDefaultAsync(p => p.ProductSerial == request.ProductSerial, cancellationToken);

        if (print is null)
        {
            return new()
            {
                Result = false 
            };
        }

        print.ProductName = request.ProductName;
        print.ProductRegCode = request.ProductRegCode;
        print.ProductWeight = request.ProductPackWeight;
        print.DocumentId = request.DocumentId;
        print.ProductProductionShift = request.ProductProductionShift;
        print.ProductProductionLine = request.ProductProductionLine;
        print.DestinationCode = request.DestinationCode;
        print.ProductStatus = request.ProductStatusCode;
        print.ProductProperties = request.ProductProperties;
        print.ProductCount = request.ProductCount;

        context.Update(print);

        return new()
        {
            Result = (await context.SaveChangesAsync(cancellationToken)) > 0
        };
    }
}
