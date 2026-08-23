namespace Silo.Api.External.Sharif.Features.LoanRenewal;

public class RenewLoanRequest
{
    public List<string> RfidUids { get; set; } = new();
}
