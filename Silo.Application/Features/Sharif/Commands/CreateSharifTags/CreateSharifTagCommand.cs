namespace Silo.Application.Features;

public class CreateSharifTagCommand : IRequest<CreateSharifTagVm>
{
    public List<string> Epcs { get; set; } = new();
    public string? GateType { get; set; }
    public string? StationCode { get; set; }
}
