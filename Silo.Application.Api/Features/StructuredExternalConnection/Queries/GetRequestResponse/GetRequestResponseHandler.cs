using System.ComponentModel.DataAnnotations;
using Silo.Application.Dto;
using Silo.Application.Exceptions;

namespace Silo.Application.Features;

public class GetRequestResponseHandler(WmsApiContext context)
    : IRequestHandler<GetRequestResponseQuery, GetRequestResponseVm>
{
    public async Task<GetRequestResponseVm> Handle(GetRequestResponseQuery request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationResult>();
      
        if (!Validator.TryValidateObject(request, new ValidationContext(request), errors, true))
        {
            throw new SiloValidationException(errors);
        }

        var connection = await context.StructuredExternalConnections
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(connection => connection.EventId == request.CommandUniqueId, cancellationToken);

        if (connection is null || connection.Response is null)
        {
            throw new MethodNotFoundException("اطلاعات یافت نشد");
        }

       
        return new GetRequestResponseVm()
        {
            Result = JsonSerializer.Deserialize<ApiResponseWithStatusCode<JsonElement>>(connection.Response)
                ?? throw new JsonException()
        };
    }
}
