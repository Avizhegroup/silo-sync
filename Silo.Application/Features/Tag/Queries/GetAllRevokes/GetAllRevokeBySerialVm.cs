namespace Silo.Application.Features;

public class GetAllRevokeBySerialVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductRegCode { get; set; }
    public string ProductName { get; set; }
    public decimal ProductCount { get; set; }
    public string RegisterUser { get; set; }
    public string RegisterDate { get; set; }
    public string SoftDeleteUser { get; set; }
    public string SoftDeleteDate { get; set; }
}
