namespace Silo.Application.Features;

public class GetOpenSyncFailuresQuery : IRequest<List<GetOpenSyncFailuresVm>>
{
    public string? Status { get; set; }
}
