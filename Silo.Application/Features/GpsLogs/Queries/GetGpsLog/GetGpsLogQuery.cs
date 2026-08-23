namespace Silo.Application.Features;

public class GetGpsLogQuery : IRequest<GetGpsLogVm>
{
    public string UsageId { get; set; }
}
