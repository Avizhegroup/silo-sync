namespace Silo.Application.Dto;

public class SearchApiSyncSto
{
    public string WarehouseCode { get; set; }
    public string Date { get; set; }
    public string GateCode { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductType { get; set; }
    public string ProductQc { get; set; }
    public string ProductSize { get; set; }
    public int? ActionType { get; set; }
}

public class SendActionToApiDto
{
    public string WarehouseCode { get; set; }
    public string DocumentType { get; set; }
    public string BasicDocument { get; set; }
}
