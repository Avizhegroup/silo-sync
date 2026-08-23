using Microsoft.AspNetCore.Http;
using Silo.Application.Exceptions;
using Silo.Shared.Tools;

namespace Silo.Application.Api.Features;
public class TransferDocumentHandler(WmsApiContext apiContext, IWmsBusiness business, IHttpContextAccessor httpContext) : IRequestHandler<TransferDocumentCommand,TransferDocumentVm>
{
    public async Task<TransferDocumentVm> Handle(TransferDocumentCommand request, CancellationToken cancellationToken)
    {
        TransferDocumentValidator validator = new();
        validator.Validate(request);

        var originalFileName = $"TriggerApi_{request.DocumentHeader.DocumentKey}{Guid.NewGuid().ToString().Substring(0, 6)}";

        var jsonArray = TransformToJsonElements(request.DocumentHeader, request.DocumentItems);

        List<InputFileData> inputs = new();

        foreach (var item in jsonArray.EnumerateArray())
        {
            inputs.Add(new()
            {
                DateTime = DateTime.Now,
                FileName = originalFileName,
                Type = request.DocumentHeader.DocumentType.ToString(),
                Type1 = request.DocumentHeader.DocumentType1.ToString(),
                Type2 = request.DocumentHeader.DocumentType2.ToString(),
                Data = JsonTools.ConvertJsonElementToEncodedString(item),
                User = httpContext.HttpContext.User.GetUserId()
            });
        }

        await apiContext.AddRangeAsync(inputs);

        if (await apiContext.SaveChangesAsync() > 0)
        {
            var result = business.SDocumentDataFromInputFile(
                originalFileName,
                $"{request.DocumentHeader.DocumentType},{request.DocumentHeader.DocumentType1},{request.DocumentHeader.DocumentType2}",
                request.DocumentHeader.DocumentCheckType);

            if (result)
            {
                return new TransferDocumentVm
                {
                    Result = true
                };
            }
        }

        throw new MethodExecutionFailedException();
    }

    private static JsonElement TransformToJsonElements(TransferDocumentHeaderDto header, List<TransferDocumentItemDto> items)
    {
        List<JsonElement> resultArray = new ();

        foreach (var item in items)
        {
            var itemObject = new Dictionary<string, object>
            {
                { nameof(header.DocumentKey), header.DocumentKey },
                { nameof(header.DocumentType), header.DocumentType },
                { nameof(header.DocumentType1), header.DocumentType1 },
                { nameof(header.DocumentType2), header.DocumentType2 },
                { nameof(header.Description), header.Description },
                { nameof(item.ProductCode), item.ProductCode },
                { nameof(item.ProductTitle), item.ProductTitle },
                { nameof(item.Count), item.Count },
                { nameof(item.ProductUnit), item.ProductUnit }
            };

            if (header.HeaderData != null)
            {
                foreach (var element in header.HeaderData.RootElement.EnumerateObject())
                {
                    itemObject[element.Name] = element.Value;
                }
            }
            else
            {
                itemObject[nameof(header.HeaderData)] = null;
            }

            if (item.ItemData != null)
            {
                foreach (var element in item.ItemData.RootElement.EnumerateObject())
                {
                    itemObject[element.Name] = element.Value;
                }
            }
            else
            {
                itemObject[nameof(item.ItemData)] = null;
            }

            var itemJson = JsonSerializer.SerializeToElement(itemObject);
            resultArray.Add(itemJson);
        }

        return JsonSerializer.SerializeToElement(resultArray);
    }
}
