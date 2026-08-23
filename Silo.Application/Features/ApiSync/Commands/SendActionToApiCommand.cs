namespace Silo.Application.Features;

public class SendActionToApiCommand
{
    public string WarehouseCode { get; set; }
    public string DocumentType { get; set; }
    public string BasicDocument { get; set; }
}
