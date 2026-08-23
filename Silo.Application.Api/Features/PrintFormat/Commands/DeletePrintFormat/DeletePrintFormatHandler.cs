namespace Silo.Application.Api.Features;
public class DeletePrintFormatHandler(WmsApiContext context)
    : IRequestHandler<DeletePrintFormatCommand, DeletePrintFormatVm>
{
    public async Task<DeletePrintFormatVm> Handle(DeletePrintFormatCommand request, CancellationToken cancellationToken)
    => new DeletePrintFormatVm
    {
        Result = (await context.PrintFormats
                               .Where(p => p.Id == request.Id)
                               .ExecuteDeleteAsync(cancellationToken)) > 0
    };
}
