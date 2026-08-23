
namespace Silo.Application.Shared.Features;
public class UpdateDestinationTypeCommand : IRequest<UpdateDestinationTypeVm>
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
}
