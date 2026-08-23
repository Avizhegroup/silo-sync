namespace Silo.Application.Features;
public class DeletePrintFormatCommand : IRequest<DeletePrintFormatVm>
{
    public int Id { get; set; }
}
