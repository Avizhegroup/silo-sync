using Silo.Api.External.Sharif.Models;
using Silo.Api.External.Sharif.Services;

namespace Silo.Application.Api.Features;

public class CreateNewTagsHandler : IRequestHandler<CreateSharifTagCommand, CreateSharifTagVm>
{
    private readonly IWmsBusiness _wmsBusiness;
    private readonly SharifExternalConnect _sharifExternalConnect;

    public CreateNewTagsHandler(
        IWmsBusiness wmsBusiness,
        SharifExternalConnect sharifExternalConnect)
    {
        _wmsBusiness = wmsBusiness;
        _sharifExternalConnect = sharifExternalConnect;
    }

    public async Task<CreateSharifTagVm> Handle(
        CreateSharifTagCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Epcs == null || request.Epcs.Count == 0)
        {
            return new CreateSharifTagVm { Result = false };
        }

        var operationCode = _wmsBusiness.CreateUhfReaderLogHeader(
            request.StationCode ?? "",
            request.GateType ?? "",
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


        var snapshotRequest = new RfidSnapshotRequest
        {
            KioskId = request.StationCode ?? string.Empty,
            ReaderId = string.Empty,
            SequenceNo = operationCode,
            CapturedAt = DateTime.UtcNow,
            Tags = request.Epcs
                 .Distinct()
                 .Select(epc => new RfidSnapshotTag
                  {
                 Uid = epc
                  })
                 .ToList()
         };

        await _sharifExternalConnect.SendRegisterTagToExternalApi(snapshotRequest, cancellationToken);

        return new CreateSharifTagVm
        {
            Result = result
        };
    }
}
