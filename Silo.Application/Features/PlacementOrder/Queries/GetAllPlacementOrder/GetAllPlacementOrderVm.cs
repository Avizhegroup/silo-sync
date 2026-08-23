namespace Silo.Application.Features;

public class GetAllPlacementOrderVm
{
    public int OperationCode { get; set; }
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
    public DateTime DateTime { get; set; }
    public string User { get; set; }
    public int Status { get; set; }
    public int OrderCount { get; set; }
}
