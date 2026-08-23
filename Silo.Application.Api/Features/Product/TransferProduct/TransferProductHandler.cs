using System.Text.Json.Nodes;
using Silo.Application.Exceptions;

namespace Silo.Application.Api.Features;
public class TransferProductHandler(IWmsBusiness wmsBusiness, IMapper mapper) : IRequestHandler<TransferProductCommand, TransferProductVm>
{
    public async Task<TransferProductVm> Handle(TransferProductCommand request, CancellationToken cancellationToken)
    {
        TransferProductValidator validator = new();
        validator.Validate(request);

        var originalFileName = $"TriggerApi_{Guid.NewGuid().ToString().Substring(0, 6)}";

        List<SaveNonDocFileCommand> commands = new();

        List<SaveProductTerchnicalDataCommand> technicalDataCommands = new();

        foreach (var productDto in request.NewProducts)
        {
            SaveProductCommand newProduct = mapper.Map<SaveProductCommand>(productDto);
            newProduct.IsActive = true;

            commands.Add(new SaveNonDocFileCommand
            {
                Type = (int)NonDocFileTypeEnum.ProductData,
                FileName = originalFileName,
                Data = JsonSerializer.Serialize(newProduct)
            });

            if (productDto.ProductTechnicalData is not null
                && (productDto.ProductTechnicalData.RootElement.ValueKind == JsonValueKind.Object
                && productDto.ProductTechnicalData.RootElement.EnumerateObject().Any()))
            {
                var technicalData = JsonNode.Parse(productDto.ProductTechnicalData.RootElement.GetRawText()) as JsonObject;

                technicalDataCommands.Add(new()
                {
                    Data = technicalData,
                    ProductCode = newProduct.ProductCode
                });
            }
        }

        var result = wmsBusiness.SSaveNdfLog(commands);

        if (result)
        {
            wmsBusiness.SSaveTechnicalInformationUsingJsonArray(technicalDataCommands);

            return new TransferProductVm
            {
                Result = result
            };
        }

        throw new MethodExecutionFailedException();
    }

}
