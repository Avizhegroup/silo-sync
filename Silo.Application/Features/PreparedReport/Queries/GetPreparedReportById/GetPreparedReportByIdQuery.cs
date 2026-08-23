namespace Silo.Application.Features;
public class GetPreparedReportByIdQuery : IRequest<GetPreparedReportByIdVm>
{
    public int Id { get; set; }
}
