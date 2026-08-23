namespace Silo.Application.Features;
public class DeleteActionTypeByIdCommand : IRequest<DeleteActionTypeByIdVm>
{
    public int Id { get; set; }
}
