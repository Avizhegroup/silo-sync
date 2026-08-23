using Microsoft.EntityFrameworkCore;

namespace Silo.Application.Features;
public class GetUserTokensHandler(WmsApiContext context)
    : IRequestHandler<GetUserTokensQuery, GetUserTokensVm>
{
    public async Task<GetUserTokensVm> Handle(
        GetUserTokensQuery request,
        CancellationToken cancellationToken)
    => new()
    {
        Result = await context.UserTokens
            .Where(ut => ut.UserId == request.UserId)
            .OrderByDescending(ut => ut.Id)
            .Select(ut => new GetUserTokensDto
            {
                Id = ut.Id,
                Value = ut.Value,
                UserId = ut.UserId,
                HasExpired = ut.HasExpired
            })
            .ToListAsync(cancellationToken)
    };
}
