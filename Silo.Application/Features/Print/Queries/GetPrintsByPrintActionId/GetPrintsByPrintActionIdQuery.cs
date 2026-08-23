namespace Silo.Application.Features;

public class GetPrintsByPrintActionIdQuery : IRequest<GetPrintsByPrintActionIdVm>
{
    public int ActionId { get; set; }
}
