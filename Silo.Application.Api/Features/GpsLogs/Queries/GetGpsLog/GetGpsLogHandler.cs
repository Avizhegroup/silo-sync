namespace Silo.Application.Api.Features;
public class GetGpsLogHandler(WmsApiContext apiContext
    , IMapper mapper) : IRequestHandler<GetGpsLogQuery, GetGpsLogVm>
{
    public async Task<GetGpsLogVm> Handle(GetGpsLogQuery request, CancellationToken cancellationToken)
    {
        return new()
        {
            List = mapper.Map<List<GetGpsLogDto>>(apiContext.GpsLogs.Where(p => p.UsageId == request.UsageId)
                                                                    .Include(p => p.User)
                                                                    .OrderByDescending(p => p.LogDateTime))
        };
    }
}
