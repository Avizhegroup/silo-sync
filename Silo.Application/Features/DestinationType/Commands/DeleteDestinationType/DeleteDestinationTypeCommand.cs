namespace Silo.Application.Shared.Features;
public class DeleteDestinationTypeCommand: IRequest<DeleteDestinationTypeVm>
{
    public int Id { get; set; }
    public int Code { get; set; }

}
