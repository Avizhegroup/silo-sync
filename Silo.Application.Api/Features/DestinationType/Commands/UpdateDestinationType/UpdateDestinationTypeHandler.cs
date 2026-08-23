using Newtonsoft.Json;
using Silo.Application.Shared.Features;
using Silo.Domains.Entities;

namespace Silo.Application.Api.Features;
public class UpdateDestinationTypeHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<UpdateDestinationTypeCommand, UpdateDestinationTypeVm>
{
    public async Task<UpdateDestinationTypeVm> Handle(UpdateDestinationTypeCommand request, CancellationToken cancellationToken)
    {
        var destinationtype = (await context.WarehouseTypes
                            .FirstOrDefaultAsync(p => p.Id == request.Id));

        destinationtype.Code = request.Code;
        destinationtype.Title = request.Title;


        context.WarehouseTypes.Update(destinationtype);

        return new()
        {
            Result = await context.SaveChangesAsync(cancellationToken) > 0

        };
    }

}
