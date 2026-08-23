
namespace Silo.Application.Shared.Features;
public class GetAllDestinationTypeQuery: IRequest<GetAllDestinationTypeVm>
{
    public string Code { get; set; }
    public string Title { get; set; }
}
