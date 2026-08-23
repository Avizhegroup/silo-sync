namespace Silo.Application.Api.Features;
public class CreatePrintFormatHandler(WmsApiContext context)
    : IRequestHandler<CreatePrintFormatCommand, CreatePrintFormatVm>
{
    public async Task<CreatePrintFormatVm> Handle(CreatePrintFormatCommand request, CancellationToken cancellationToken)
    {
        var printFormat = new PrintFormat
        {
            Name = request.Name,
            PageTitle = request.PageTitle,
            Path = request.Path
        };

        if (request.Id.HasValue && request.Id > 0)
        {
            printFormat.Id = request.Id.Value;
            context.PrintFormats.Update(printFormat);
        }
        else
        {
            await context.PrintFormats.AddAsync(printFormat, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return new CreatePrintFormatVm
        {
            Result = printFormat.Id
        };
    }
}
