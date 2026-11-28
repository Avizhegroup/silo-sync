using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

public class SetPrintFormatHandler : IRequestHandler<SetPrintFormatCommand, SetPrintFormatVm>, ITeamYarSecHandler
{
    public Task<SetPrintFormatVm> Handle(SetPrintFormatCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
