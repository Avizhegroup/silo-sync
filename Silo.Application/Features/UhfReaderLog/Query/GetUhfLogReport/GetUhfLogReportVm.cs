namespace Silo.Application.Features;
public class GetUhfLogReportVm
{
    public string InventoryId { get; set; }
    public string ReaderGateCode { get; set; }
    public string ReaderGateName { get; set; }
    public string Username { get; set; }
    public string DateTime { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string ActionDesc { get; set; }
    public int CountTag { get; set; }
    public int CountTagOk { get; set; }
    public decimal SumValue { get; set; }
    public string ActionStatus { get; set; }
}
    

