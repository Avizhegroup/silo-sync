using Silo.Application.Shared.Features;
using Silo.Domains.Entities;

namespace Silo.Application.Api.Features;
public class CreateNewDestinationTypeHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<CreateNewDestinationTypeCommand, CreateNewDestinationTypeVm>
{
    public async Task<CreateNewDestinationTypeVm> Handle(CreateNewDestinationTypeCommand request, CancellationToken cancellationToken)
    {
        var destinationtype = mapper.Map<Silo.Domains.Entities.WarehouseType>(request);
        destinationtype.Code = request.Code;
        destinationtype.Title = request.Title;

        context.WarehouseTypes.Update(destinationtype);

        await context.SaveChangesAsync(cancellationToken);

        return new CreateNewDestinationTypeVm
        {
            Result = destinationtype.Id

        };


    }




}
