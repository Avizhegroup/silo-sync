namespace Silo.Api.External.Sharif.Features.ExitPermission;

public class CheckExitPermissionRequest
{
    public List<string> RfidUids { get; set; } = new();
    public string GateId { get; set; } = string.Empty;
}
