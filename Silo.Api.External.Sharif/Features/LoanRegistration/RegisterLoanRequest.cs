namespace Silo.Api.External.Sharif.Features.LoanRegistration;

public class RegisterLoanRequest
{
    public string MemberBarcode { get; set; } = string.Empty;
    public List<string> RfidUids { get; set; } = new();
    public string OptHashValue { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
}
