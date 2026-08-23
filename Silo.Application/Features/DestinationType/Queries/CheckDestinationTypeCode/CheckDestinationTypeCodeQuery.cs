
namespace Silo.Application.Shared.Features;
public class CheckDestinationTypeCodeQuery : IRequest<CheckDestinationTypeCodeVm>
{
    public string Code { get; set; }

}
