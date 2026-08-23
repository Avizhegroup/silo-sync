
namespace Silo.Application.Api.Features;

public class CheckActionTypeCodeHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetActionTypeByCodeQuery, GetActionTypeByCodeVm>
{
    public async Task<GetActionTypeByCodeVm> Handle(GetActionTypeByCodeQuery request, CancellationToken cancellationToken)
        => new()
        {
            Result = (await context.ActionTypes.AnyAsync(p => p.Code == request.Code))

        };
}
