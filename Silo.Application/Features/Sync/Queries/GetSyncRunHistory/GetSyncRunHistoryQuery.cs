namespace Silo.Application.Features;

public class GetSyncRunHistoryQuery : IRequest<List<GetSyncRunHistoryVm>>
{
    public string? SourceKey { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
