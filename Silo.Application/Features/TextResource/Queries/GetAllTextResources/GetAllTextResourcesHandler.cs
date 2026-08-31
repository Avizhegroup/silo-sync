
namespace Silo.Application.Features;

public class GetAllTextResourcesHandler(WmsApiContext context)
    : IRequestHandler<GetAllTextResourcesQuery, List<GetAllTextResourcesVm>>
{
    public async Task<List<GetAllTextResourcesVm>> Handle(GetAllTextResourcesQuery request
        , CancellationToken cancellationToken)
    {
        return await context.TextResources
            .AsNoTracking()
            .Select(x => new GetAllTextResourcesVm
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            })
            .ToListAsync(cancellationToken);
    }
}
