namespace Silo.Application.Api.Features;

public class GetChatSessionByIdHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<GetChatSessionByIdQuery, GetChatSessionByIdVm>
{
    public async Task<GetChatSessionByIdVm> Handle(GetChatSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await context.ChatSessions
            .FirstOrDefaultAsync(s => s.SessionId == request.SessionId && s.UserId == request.UserId, cancellationToken);

        return new GetChatSessionByIdVm
        {
            Session = session is null ? null : mapper.Map<GetAllChatSessionsDto>(session)
        };
    }
}
