namespace Silo.Api.External.Sharif.Features.LoanRenewal;

public class RenewLoanCommand
{
    public List<string> RfidUids { get; set; } = new();
}
