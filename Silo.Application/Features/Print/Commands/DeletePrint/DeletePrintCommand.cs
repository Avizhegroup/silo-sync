namespace Silo.Application.Features;

public class DeletePrintCommand : IRequest<DeletePrintVm>
{
    public string ProductSerial { get; set; }
}
