using Silo.Application.Shared.Features;

namespace Silo.Application.Api.Features;
public class GetAllDestinationTypeHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetAllDestinationTypeQuery, GetAllDestinationTypeVm>
    {
        public async Task<GetAllDestinationTypeVm> Handle(GetAllDestinationTypeQuery request
            , CancellationToken cancellationToken)

        {
            var List = mapper.Map<List<GetAllDestinationTypeDto>>(context.WarehouseTypes);
            return new GetAllDestinationTypeVm()
            {
                List = List
            };

        }
 }


