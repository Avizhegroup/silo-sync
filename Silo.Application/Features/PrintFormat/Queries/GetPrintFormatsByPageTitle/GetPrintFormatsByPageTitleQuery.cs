namespace Silo.Application.Features;
public class GetPrintFormatsByPageTitleQuery : IRequest<GetPrintFormatsByPageTitleVm>
{
    public string PageTitle { get; set; }
}
