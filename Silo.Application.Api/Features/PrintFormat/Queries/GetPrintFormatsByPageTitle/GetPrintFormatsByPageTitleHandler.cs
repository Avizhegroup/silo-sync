namespace Silo.Application.Api.Features;

public class GetPrintFormatsByPageTitleHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<GetPrintFormatsByPageTitleQuery, GetPrintFormatsByPageTitleVm>
{
    public async Task<GetPrintFormatsByPageTitleVm> Handle(GetPrintFormatsByPageTitleQuery request, CancellationToken cancellationToken)
    {
        if (request.ActionTypeId.HasValue)
        {
            var listWithAction = await context.PrintFormats
                .Where(p => (p.PageTitle == request.PageTitle
                             || p.PageTitle.StartsWith(request.PageTitle + "|")
                             || p.PageTitle.EndsWith("|" + request.PageTitle)
                             || p.PageTitle.Contains("|" + request.PageTitle + "|"))
                            && p.ActionTypeId == request.ActionTypeId.Value)
                .ToListAsync(cancellationToken);

            if (listWithAction.Any())
            {
                return new()
                {
                    List = mapper.Map<List<GetPrintFormatsByPageTitleDto>>(listWithAction)
                };
            }
        }

        var list = await context.PrintFormats
                         .Where(p => p.PageTitle == request.PageTitle
                                 || p.PageTitle.StartsWith(request.PageTitle + "|")
                                 || p.PageTitle.EndsWith("|" + request.PageTitle)
                                 || p.PageTitle.Contains("|" + request.PageTitle + "|"))
                         .ToListAsync(cancellationToken);

        return new()
        {
            List = mapper.Map<List<GetPrintFormatsByPageTitleDto>>(list)
        };
    }
}
