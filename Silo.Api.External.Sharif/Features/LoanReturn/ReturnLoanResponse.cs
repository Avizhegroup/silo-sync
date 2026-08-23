namespace Silo.Api.External.Sharif.Features.LoanReturn;

public class ReturnLoanResponse
{
    public List<ReturnItemResult> Items { get; set; } = new();
}

public class ReturnItemResult
{
    public string RfidUid { get; set; } = string.Empty;
    public string LoanableItemId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Message { get; set; } = string.Empty;
}
