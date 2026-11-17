namespace Silo.Application.Features;

public class AddBulkUserClaimsCommand : IRequest<AddBulkUserClaimsVm>
{
    public List<string> UserIds { get; set; } = new();

    public List<Claim> Claims { get; set; } = new();
}
