namespace Silo.Application.Features;

public class SearchInventoryConflictDetailsDto
{
    public string ProductSerial { get; set; }
    public string Location { get; set; }
    public string ContractStatus { get; set; }
    public string FromRegisterDate { get; set; }
    public string ToRegisterDate { get; set; }
    public string Warehouse { get; set; }
}
