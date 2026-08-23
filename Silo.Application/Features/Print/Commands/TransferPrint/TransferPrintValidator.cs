using Silo.Application.Contracts;
using Silo.Application.Exceptions;
using Silo.Domains.Services;

namespace Silo.Application.Features;
public class TransferPrintValidator(WmsApiContext apiContext) : ISiloBaseValidator<TransferPrintCommand>
{
    public void Validate(TransferPrintCommand request)
    {
        List<ValidationResult> validationResults = new();

        var context = new ValidationContext(request, serviceProvider: null, items: null);

        bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

        if (!isValid)
        {
            throw new SiloValidationException(validationResults);
        }

        var product = apiContext.Products.FirstOrDefault(p => p.Code == request.ProductCode);

        if (product is null)
        {
            throw new ProductNotFoundException(new());
        }
    }
}
