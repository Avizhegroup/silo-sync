namespace Silo.Application.Features;

public class CreateSharifTagCommand : IRequest<CreateSharifTagVm>
{
    public string Epc { get; set; }
    public string? GateType { get; set; }
    public string? StationCode { get; set; }
}
