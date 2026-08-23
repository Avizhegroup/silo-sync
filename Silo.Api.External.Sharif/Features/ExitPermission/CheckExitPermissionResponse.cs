namespace Silo.Api.External.Sharif.Features.ExitPermission;

public class CheckExitPermissionResponse
{
    public bool CanExit { get; set; }
    public List<ExitCheckItemResult> Items { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class ExitCheckItemResult
{
    public string RfidUid { get; set; } = string.Empty;
    public string LoanableItemId { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
    public string Reason { get; set; } = string.Empty;
}
