
using Silo.Application.Shared.Features;

namespace Silo.Application.Api.Features;
public class CheckDestinationTypeCodeHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<CheckDestinationTypeCodeQuery, CheckDestinationTypeCodeVm>
{
    public async Task<CheckDestinationTypeCodeVm> Handle(CheckDestinationTypeCodeQuery request, CancellationToken cancellationToken)
        => new()
        {
            Result = (await context.WarehouseTypes.AnyAsync(p => p.Code == request.Code))

        };
}
