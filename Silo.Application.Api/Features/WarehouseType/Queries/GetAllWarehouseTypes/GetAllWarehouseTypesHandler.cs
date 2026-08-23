namespace Silo.Application.Api.Features;
public class GetAllWarehouseTypesHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetAllWarehouseTypesQuery, GetAllWarehouseTypesVm>
{
    public async Task<GetAllWarehouseTypesVm> Handle(GetAllWarehouseTypesQuery request, CancellationToken cancellationToken)
    => new()
    {
        List = mapper.Map<List<GetAllWarehouseTypesDto>>(context.WarehouseTypes)
    };
}
