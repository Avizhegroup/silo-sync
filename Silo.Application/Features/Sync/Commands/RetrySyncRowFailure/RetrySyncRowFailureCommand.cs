namespace Silo.Application.Features;

public class RetrySyncRowFailureCommand : IRequest<RetrySyncRowFailureVm>
{
    public int Id { get; set; }
}
