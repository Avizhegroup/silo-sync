namespace Silo.Application.Features;

public class DeletePrintHandler(WmsApiContext context)
    : IRequestHandler<DeletePrintCommand, DeletePrintVm>
{
    public async Task<DeletePrintVm> Handle(DeletePrintCommand request, CancellationToken cancellationToken)
    => new()
    {
        Result = (await context.Prints.Where(p => p.ProductSerial == request.ProductSerial).ExecuteDeleteAsync(cancellationToken)) > 0
    };
}
