using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Services;

namespace Silo.Application.Api.Features.Sync;

public class GetSyncRunHistoryHandler(WmsApiContext context)
    : IRequestHandler<GetSyncRunHistoryQuery, List<GetSyncRunHistoryVm>>
{
    public async Task<List<GetSyncRunHistoryVm>> Handle(GetSyncRunHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = context.SyncRunLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SourceKey))
        {
            query = query.Where(x => x.SourceKey == request.SourceKey);
        }

        if (request.From.HasValue)
        {
            query = query.Where(x => x.StartedAt >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(x => x.StartedAt <= request.To.Value);
        }

        return await query
            .OrderByDescending(x => x.StartedAt)
            .Select(x => new GetSyncRunHistoryVm
            {
                Id = x.Id,
                SourceKey = x.SourceKey,
                StartedAt = x.StartedAt,
                FinishedAt = x.FinishedAt,
                RowsFetched = x.RowsFetched,
                RowsSucceeded = x.RowsSucceeded,
                RowsFailed = x.RowsFailed,
                Status = x.Status,
                ErrorSummary = x.ErrorSummary
            })
            .ToListAsync(cancellationToken);
    }
}
