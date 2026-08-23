
namespace Silo.Application.Api.Features;

public class GetAllActionTypesHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetAllActionTypesQuery, GetAllActionTypesVm>
{
    public async Task<GetAllActionTypesVm> Handle(GetAllActionTypesQuery request
        , CancellationToken cancellationToken)

    {
        var List = mapper.Map<List<GetAllActionTypesDto>>(context.ActionTypes);
        return new GetAllActionTypesVm()
        {
            List = List
        };

    }
}

