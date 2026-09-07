namespace Silo.Application.Features;

public class RetrySyncRowFailureVm
{
    public bool Success { get; set; }
    public string? ErrorCategory { get; set; }
    public string? ErrorMessage { get; set; }
}
