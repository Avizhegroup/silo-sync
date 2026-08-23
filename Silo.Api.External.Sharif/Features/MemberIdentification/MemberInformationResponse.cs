namespace Silo.Api.External.Sharif.Features.MemberIdentification;

public class MemberInformationResponse
{
    public string Id { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string OptHashValue { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
}
