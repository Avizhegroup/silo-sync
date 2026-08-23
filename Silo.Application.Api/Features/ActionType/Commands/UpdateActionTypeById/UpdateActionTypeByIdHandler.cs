using Newtonsoft.Json;

namespace Silo.Application.Api.Features;
public class UpdateActionTypeByIdHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<UpdateActionTypeByIdCommand, UpdateActionTypeByIdVm>
{
    public async Task<UpdateActionTypeByIdVm> Handle(UpdateActionTypeByIdCommand request, CancellationToken cancellationToken)
    {

        var actionType = (await context.ActionTypes
                             .FirstOrDefaultAsync(p => p.Id == request.Id));

        List<ActionTypeControls>? activeControls = await context.ActionTypeControls.ToListAsync();
        Dictionary<string?, bool>? activeControlsDict = activeControls.ToDictionary
           (
           control => control.Code,
           control => request.ChoosenActionControls.Contains(control.Code)
           );

        actionType.Code = request.Code;
        actionType.Title = request.Title;
        actionType.From = $",{string.Join(",", request.ChoosenFromWarehouseTypes)}";
        actionType.To = $",{string.Join(",", request.ChoosenToWarehouseTypes)}";
        actionType.DocStatusPermitted = $",{string.Join(",", request.ChoosenDocumentPermittedStatuses)}";
        actionType.DocStatusChange = string.Join(",", request.ChoosenDocumentChangeStatuses);
        actionType.RFIDPower = request.RfidPower;

        actionType.ActiveControls = JsonConvert.SerializeObject(activeControlsDict);


        context.ActionTypes.Update(actionType);

        return new()
        {
            Result = await context.SaveChangesAsync(cancellationToken) > 0

        };
    }


}
