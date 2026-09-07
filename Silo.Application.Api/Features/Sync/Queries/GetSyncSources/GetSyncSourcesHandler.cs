using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Services;

namespace Silo.Application.Api.Features.Sync;

public class GetSyncSourcesHandler(WmsApiContext context)
    : IRequestHandler<GetSyncSourcesQuery, List<GetSyncSourcesVm>>
{
    public async Task<List<GetSyncSourcesVm>> Handle(GetSyncSourcesQuery request, CancellationToken cancellationToken)
    {
        return await context.SyncSourceConfigs
            .AsNoTracking()
            .OrderBy(x => x.SourceKey)
            .Select(x => new GetSyncSourcesVm
            {
                Id = x.Id,
                SourceKey = x.SourceKey,
                DisplayName = x.DisplayName,
                SourceType = x.SourceType,
                Command = x.Command ?? string.Empty,
                FieldKey = x.FieldKey ?? string.Empty,
                FieldCheck = x.FieldCheck ?? string.Empty,
                FieldOrder = x.FieldOrder ?? string.Empty,
                IntervalSeconds = x.IntervalSeconds,
                IsEnabled = x.IsEnabled,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate
            })
            .ToListAsync(cancellationToken);
    }
}
