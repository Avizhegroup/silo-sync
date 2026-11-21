using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

public class RevokeTagForTeamYarHandler : IRequestHandler<RevokeTagForTeamYarCommand, RevokeTagForTeamYarVm>, ITeamYarSecHandler
{
    public Task<RevokeTagForTeamYarVm> Handle(RevokeTagForTeamYarCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
