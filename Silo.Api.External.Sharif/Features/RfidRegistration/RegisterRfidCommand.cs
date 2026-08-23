namespace Silo.Api.External.Sharif.Features.RfidRegistration;

public class RegisterRfidCommand
{
    public string LoanableItemId { get; set; } = string.Empty;
    public string RfidUid { get; set; } = string.Empty;
}
