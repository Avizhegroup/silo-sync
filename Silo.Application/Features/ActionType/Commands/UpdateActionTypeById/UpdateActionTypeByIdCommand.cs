
namespace Silo.Application.Features;

public class UpdateActionTypeByIdCommand : IRequest<UpdateActionTypeByIdVm>
{
    public int Id { get; set; }
    public int? Code { get; set; }
    public string? Title { get; set; }
    public int? RfidPower { get; set; }
    public List<string> ChoosenFromWarehouseTypes { get; set; } = new();
    public List<string> ChoosenToWarehouseTypes { get; set; } = new();
    public List<int> ChoosenDocumentChangeStatuses { get; set; } = new();
    public List<int> ChoosenDocumentPermittedStatuses { get; set; } = new();
    public List<string> ChoosenActionControls { get; set; } = new();

}
