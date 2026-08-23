using Silo.Application.Exceptions;

namespace Silo.Application.Features;
public class TransferProductValidator
{
    public void Validate(TransferProductCommand request)
    {
        foreach (var product in request.NewProducts)
        {
            List<ValidationResult> validationResults = new();

            var context = new ValidationContext(product, serviceProvider: null, items: null);

            bool isValid = Validator.TryValidateObject(product, context, validationResults, true);

            if (!isValid)
            {
                throw new SiloValidationException(validationResults);
            }
        }
    }

}
