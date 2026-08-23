namespace Silo.Application.Dto;

public class InputFileDto
{
    public List<string> Serials { get; set; }
    public string FromZone { get; set; }
    public string FromWarehouse { get; set; }
    public string DestinationZone { get; set; }
    public string DestinationWarehouse { get; set; }
}

