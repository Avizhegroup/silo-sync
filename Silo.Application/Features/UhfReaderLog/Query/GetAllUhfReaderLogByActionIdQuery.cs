namespace Silo.Application.Features;
public class GetAllUhfReaderLogByActionIdQuery
{
    public string UhfGateActionId { get; set; }
    public string UhfGateCode { get; set; }
    public string UhfGateMovementActionDestination { get; set; }
    public string UhfGateMovementActionFrom { get; set; }
    public StationTypeEnum StationType { get; set; }
}
