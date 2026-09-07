namespace Silo.Application.Features;

public class EnableDisableSyncSourceCommand : IRequest<EnableDisableSyncSourceVm>
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
}
