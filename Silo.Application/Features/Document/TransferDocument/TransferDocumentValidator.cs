using Silo.Application.Contracts;
using Silo.Application.Exceptions;

namespace Silo.Application.Features;
public class TransferDocumentValidator : ISiloBaseValidator<TransferDocumentCommand>
{
    public void Validate(TransferDocumentCommand request)
    {
        List<ValidationResult> validationResults = new();

        var contextHeader = new ValidationContext(request.DocumentHeader, serviceProvider: null, items: null);

        bool isHeaderValid = Validator.TryValidateObject(request.DocumentHeader, contextHeader, validationResults, true);

        bool isItemsValid = false;

        foreach (var item in request.DocumentItems)
        {
            var contextItem = new ValidationContext(item, serviceProvider: null, items: null);

            isItemsValid = Validator.TryValidateObject(item, contextItem, validationResults, true);

            if (!isItemsValid)
            {
                break;
            }
        }

        if (request.DocumentHeader.DocumentType == request.DocumentHeader.DocumentType1)
        {
            isHeaderValid = false;

            validationResults.Add(new(string.Format(TextResources.APP_StringKeys_Document_Type_Validation,
                TextResources.APP_StringKeys_Document_Type, TextResources.APP_StringKeys_Document_Type1)));
        }

        if (request.DocumentHeader.DocumentType == request.DocumentHeader.DocumentType2)
        {
            isHeaderValid = false;

            validationResults.Add(new(string.Format(TextResources.APP_StringKeys_Document_Type_Validation,
                TextResources.APP_StringKeys_Document_Type, TextResources.APP_StringKeys_Document_Type2)));
        }

        if ((request.DocumentHeader.DocumentType1 != 0 && request.DocumentHeader.DocumentType2 != 0) &&
            (request.DocumentHeader.DocumentType1 == request.DocumentHeader.DocumentType2))
        {
            isHeaderValid = false;

            validationResults.Add(new(string.Format(TextResources.APP_StringKeys_Document_Type_Validation,
                TextResources.APP_StringKeys_Document_Type1, TextResources.APP_StringKeys_Document_Type2)));
        }


        if (!isHeaderValid || !isItemsValid)
        {
            throw new SiloValidationException(validationResults);
        }
    }

}
