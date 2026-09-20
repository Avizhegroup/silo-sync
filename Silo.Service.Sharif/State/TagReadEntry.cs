namespace Silo.Service.Sharif.State;

public sealed class TagReadEntry
{
    public string Epc { get; init; } = string.Empty;

    public DateTime ReadUtc { get; init; }

    public string StationCode { get; init; } = string.Empty;

    public string GateType { get; init; } = string.Empty;

    public bool PostSucceeded { get; init; }

    public string? ErrorMessage { get; init; }
}
