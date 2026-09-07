namespace Silo.Application.Features;

public class DeleteSyncSourceCommand : IRequest<DeleteSyncSourceVm>
{
    public int Id { get; set; }
}
