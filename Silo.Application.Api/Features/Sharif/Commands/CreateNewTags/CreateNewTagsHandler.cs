
namespace Silo.Application.Api.Features;

public class CreateNewTagsHandler : IRequestHandler<CreateSharifTagCommand, CreateSharifTagVm>
{
    private readonly IWmsBusiness _wmsBusiness;

    public CreateNewTagsHandler(IWmsBusiness wmsBusiness)
    {
        _wmsBusiness = wmsBusiness;
    }

    public Task<CreateSharifTagVm> Handle(CreateSharifTagCommand request, CancellationToken cancellationToken)
    {
        if (request.Epc == null)
        {
            return Task.FromResult(new CreateSharifTagVm { Result = false });
        }

        var tags = new List<string> { request.Epc };

        var result = _wmsBusiness.SIdentifyPallets(
            deviceId: request.StationCode,
            listTags: tags,
            desc: "API Tag Identify",
            GateType: request.GateType,
            invCod: "1",
            doc: "0",
            DestinationCode: "1",
            userToken: "KIOSK",
            ActionDynamicData: null,
            ActionActiveControls: "",
            TruckCrossId: "",
            saveDateTime: DateTime.Now
        );

        return Task.FromResult(new CreateSharifTagVm
        {
            Result = result
        });
    }
}
