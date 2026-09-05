using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Services;

namespace Silo.Application.Api.Features.Sync;

public class GetOpenSyncFailuresHandler(WmsApiContext context)
    : IRequestHandler<GetOpenSyncFailuresQuery, List<GetOpenSyncFailuresVm>>
{
    public async Task<List<GetOpenSyncFailuresVm>> Handle(GetOpenSyncFailuresQuery request, CancellationToken cancellationToken)
    {
        var status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status;
        var query = context.SyncRowFailures
            .AsNoTracking()
            .Where(x => x.Status == status);

        return await query
            .OrderByDescending(x => x.LastAttemptAt)
            .Select(x => new GetOpenSyncFailuresVm
            {
                Id = x.Id,
                SyncRunLogId = x.SyncRunLogId,
                SourceKey = x.SourceKey,
                RowKey = x.RowKey,
                ErrorCategory = x.ErrorCategory,
                ErrorMessage = x.ErrorMessage,
                AttemptCount = x.AttemptCount,
                LastAttemptAt = x.LastAttemptAt,
                NextAttemptAt = x.NextAttemptAt,
                Status = x.Status,
                ResolvedDate = x.ResolvedDate
            })
            .ToListAsync(cancellationToken);
    }
}
