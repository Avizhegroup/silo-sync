namespace Silo.Application.Features;

public class GetPrintsByPrintActionIdHandler : IRequestHandler<GetPrintsByPrintActionIdQuery, GetPrintsByPrintActionIdVm>
{
    public Task<GetPrintsByPrintActionIdVm> Handle(GetPrintsByPrintActionIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
