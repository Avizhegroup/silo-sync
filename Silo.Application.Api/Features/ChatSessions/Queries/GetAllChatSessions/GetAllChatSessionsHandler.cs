namespace Silo.Application.Api.Features;

public class GetAllChatSessionsHandler(WmsApiContext context
    , IMapper mapper) : IRequestHandler<GetAllChatSessionsQuery, GetAllChatSessionsVm>
{
    public async Task<GetAllChatSessionsVm> Handle(GetAllChatSessionsQuery request
        , CancellationToken cancellationToken)
    {
        var sessions = await context.ChatSessions.Where(s => s.UserId == request.UserId)
            .OrderByDescending(s => s.LastUpdated)
            .ToListAsync(cancellationToken);

        var list = mapper.Map<List<GetAllChatSessionsDto>>(sessions);

        return new GetAllChatSessionsVm()
        {
            List = list
        };
    }
}

