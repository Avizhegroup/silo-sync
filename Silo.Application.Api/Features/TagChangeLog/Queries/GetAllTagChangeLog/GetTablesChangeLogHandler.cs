namespace Silo.Application.Api.Features;
public class GetTablesChangeLogHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetAllTagChangeLogQuery, GetTablesChangeLogVm>
{
    public async Task<GetTablesChangeLogVm> Handle(GetAllTagChangeLogQuery request
        , CancellationToken cancellationToken)
    {
        var list = context.TagChangeLog
                          .Include(p => p.User)
                          .Where(p => p.RecordKey == request.RecordKey && p.TableName == request.TableName)
                          .OrderBy(x => x.CreatedAt);

        return new()
        {
            List = mapper.Map<List<GetTablesChangeLogDto>>(list) 
        };
    }
}
