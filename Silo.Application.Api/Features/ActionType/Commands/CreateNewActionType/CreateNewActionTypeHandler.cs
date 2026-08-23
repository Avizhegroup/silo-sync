using System.Linq;
using Newtonsoft.Json;

namespace Silo.Application.Api.Features;
public class CreateNewActionTypeHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<CreateNewActionTypeCommand, CreateNewActionTypeVm>
{
    public async Task<CreateNewActionTypeVm> Handle(CreateNewActionTypeCommand request, CancellationToken cancellationToken)
    {
        var actionType = mapper.Map<Silo.Domains.Entities.ActionType>(request);

        List<ActionTypeControls>? activeControls = await context.ActionTypeControls.ToListAsync();

        Dictionary<string?, bool>? activeControlsDict = activeControls.ToDictionary
        (
        control => control.Code,
        control => request.ChoosenActionControls.Contains(control.Code)
        );

        actionType.ActiveControls = JsonConvert.SerializeObject(activeControlsDict);

        actionType.From = $",{string.Join(",", request.ChoosenFromWarehouseTypes)}";

        actionType.To = $",{string.Join(",", request.ChoosenToWarehouseTypes)}";

        actionType.DocStatusPermitted = $",{string.Join(",", request.ChoosenDocumentPermittedStatuses)}";

        actionType.DocStatusChange = string.Join(",", request.ChoosenDocumentChangeStatuses);

        actionType.RFIDPower = request.RfidPower ;

        if (request.Id.HasNoValue())
        {
            actionType.Code = request.Code ?? (await context.ActionTypes.MaxAsync(p => p.Code, cancellationToken) ?? 0) + 1;

            await context.ActionTypes.AddAsync(actionType, cancellationToken);
        }

        else
        {
            context.ActionTypes.Update(actionType);
        }

        await context.SaveChangesAsync(cancellationToken);

        return new CreateNewActionTypeVm
        {
            Result = actionType.Id

        };
    }
}
