namespace Silo.Api.External.Sharif.Features.LoanRegistration;

public class RegisterLoanResponse
{
    public string LoanId { get; set; } = string.Empty;
    public string MemberBarcode { get; set; } = string.Empty;
    public List<LoanItemResult> Items { get; set; } = new();
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
}

public class LoanItemResult
{
    public string RfidUid { get; set; } = string.Empty;
    public string LoanableItemId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
