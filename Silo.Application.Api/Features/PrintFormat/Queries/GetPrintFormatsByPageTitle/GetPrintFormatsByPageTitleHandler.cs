namespace Silo.Application.Api.Features;
public class GetPrintFormatsByPageTitleHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<GetPrintFormatsByPageTitleQuery, GetPrintFormatsByPageTitleVm>
{
    public async Task<GetPrintFormatsByPageTitleVm> Handle(GetPrintFormatsByPageTitleQuery request, CancellationToken cancellationToken)
    => new()
    {
        List = mapper.Map<List<GetPrintFormatsByPageTitleDto>>(
            await context.PrintFormats
                         .Where(p => p.PageTitle == request.PageTitle
                                 || p.PageTitle.StartsWith(request.PageTitle + "|")
                                 || p.PageTitle.EndsWith("|" + request.PageTitle)
                                 || p.PageTitle.Contains("|" + request.PageTitle + "|"))
                         .ToListAsync(cancellationToken))
    };
}
