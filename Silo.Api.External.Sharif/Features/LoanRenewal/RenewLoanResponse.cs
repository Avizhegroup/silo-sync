namespace Silo.Api.External.Sharif.Features.LoanRenewal;

public class RenewLoanResponse
{
    public List<RenewalItemResult> Items { get; set; } = new();
}

public class RenewalItemResult
{
    public string RfidUid { get; set; } = string.Empty;
    public string LoanableItemId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime? NewDueDate { get; set; }
    public string Message { get; set; } = string.Empty;
}
