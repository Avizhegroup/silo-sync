namespace Silo.Application.Api.Features;

public class CreateNewTagsHandler
{
    private readonly IWmsBusiness _wmsBusiness;

    public CreateNewTagsHandler(IWmsBusiness wmsBusiness)
    {
        _wmsBusiness = wmsBusiness;
    }

    public async Task<CreateSharifTagVm> Handle(
        CreateSharifTagCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Epcs is null || request.Epcs.Count == 0)
        {
            return new CreateSharifTagVm { Result = false };
        }

        var operationCode = _wmsBusiness.CreateUhfReaderLogHeader(
            request.StationCode,
            request.GateType ,
            "KIOSK");

        var result = _wmsBusiness.SIdentifyPallets(
            deviceId: request.StationCode,
            listTags: request.Epcs,
            desc: "API Tag Identify",
            GateType: request.GateType,
            invCod: operationCode.ToString(),
            doc: "0",
            DestinationCode: "1",
            userToken: "KIOSK",
            ActionDynamicData: null,
            ActionActiveControls: "",
            TruckCrossId: "",
            saveDateTime: DateTime.Now
        );

        return new CreateSharifTagVm
        {
            Result = result,
            OperationCode = operationCode
        };
    }
}
