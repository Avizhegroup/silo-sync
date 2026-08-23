using Silo.Application.Exceptions;

namespace Silo.Application.Api.Features;
public class TransferPrintHandler(IWmsBusiness wmsBusiness, WmsApiContext apiContext) : IRequestHandler<TransferPrintCommand, TransferPrintVm>
{
    public async Task<TransferPrintVm> Handle(TransferPrintCommand request, CancellationToken cancellationToken)
    {
        TransferPrintValidator validator = new(apiContext);

        validator.Validate(request);

        var product = apiContext.Products.FirstOrDefault(p => p.Code == request.ProductCode);

        var result = wmsBusiness.SSavePrintBySerial(request.Serial, request.ProductCode,  
            product.ProductValue.ToString(),"0", "0","0","1","0","","0","0" , request.CreateUser, "-1",product.ProductProperties ?? "");

        if (result!="")
        {
            return new()
            {
                Result = true
            };
        }

        throw new MethodExecutionFailedException();
    }
}
