namespace Silo.Application.Api.Features;
public class GetAllActionTypeControlsHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetAllActionTypeControlsRequest, GetAllActionTypeControlsVm>
{
    public async Task<GetAllActionTypeControlsVm> Handle(GetAllActionTypeControlsRequest request
        , CancellationToken cancellationToken)
    => new()
    {
        List = mapper.Map<List<GetAllActionTypeControlsDto>>(context.ActionTypeControls)
    };
}
