namespace Silo.Application.Features;

public class AddBulkDashboardClaimsCommand : IRequest<AddBulkUserClaimsVm>
{
    public List<string> UserIds { get; set; } = new();

    public List<string> ItemCodes { get; set; } = new();
}
