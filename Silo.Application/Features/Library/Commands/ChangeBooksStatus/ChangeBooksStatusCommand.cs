namespace Silo.Application.Features;
public class ChangeBooksStatusCommand : IRequest<ChangeBooksStatusVm>
{
    public List<string> Serials { get; set; }
    public ChangeBooksStatusEnum Status { get; set; }
}
