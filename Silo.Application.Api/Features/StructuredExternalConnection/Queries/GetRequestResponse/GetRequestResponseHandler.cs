using System.ComponentModel.DataAnnotations;
using Silo.Application.Dto;
using Silo.Application.Exceptions;

namespace Silo.Application.Features;

public class GetRequestResponseHandler(WmsApiContext context)
    : IRequestHandler<GetRequestResponseQuery, GetRequestResponseVm?>
{
    public async Task<GetRequestResponseVm?> Handle(GetRequestResponseQuery request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), errors, true))
        {
            throw new SiloValidationException(errors);
        }

        var connection = await context.StructuredExternalConnections
            .AsNoTracking()
            .Where(connection => connection.EventId == request.CommandUniqueId)
            .OrderByDescending(connection => connection.Id)
            .Select(connection => new { connection.Response })
            .FirstOrDefaultAsync(cancellationToken);

        if (connection is null)
        {
            return null;
        }

        return new GetRequestResponseVm()
        {
            Result = connection.Response is null
                ? null
                : JsonSerializer.Deserialize<ApiResponseWithStatusCode<JsonElement>>(connection.Response)
                    ?? throw new JsonException()
        };
    }
}
