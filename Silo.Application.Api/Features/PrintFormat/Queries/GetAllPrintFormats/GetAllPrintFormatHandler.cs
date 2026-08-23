namespace Silo.Application.Api.Features;
public class GetAllPrintFormatHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<GetAllPrintFormatQuery, GetAllPrintFormatVm>
{
    public async Task<GetAllPrintFormatVm> Handle(GetAllPrintFormatQuery request, CancellationToken cancellationToken)
    => new()
    {
        List = mapper.Map<List<GetAllPrintFormatDto>>(context.PrintFormats)
    };
}
