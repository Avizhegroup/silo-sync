
namespace Silo.Application.Features;
public class GetAllActionTypesQuery : IRequest<GetAllActionTypesVm>
{
    public string Code { get; set; }
    public string Title { get; set; }

}
