namespace Silo.Application.Dto;

public class CeramicAggregateResult
{
    public string Error { get; set; }
    public string TagEpc { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductUnit { get; set; }
    public string Regcode { get; set; }
    public decimal ProductCount { get; set; }
    public decimal SumCount { get; set; }
    public string MaxDate { get; set; }
    public string SourceWarehouseCode { get; set; }
    public string StationName { get; set; }
}

public class RegisterStats
{
    public List<RegisterStatsDetails> Reg { get; set; }
}

public class ExitStats
{
    public List<RegisterStatsDetails> Exit { get; set; }
}

public class RegisterStatsDetails
{
    public decimal Avg { get; set; }
    public decimal Max { get; set; }
    public RegisterStatsEnum Type { get; set; }
}


public enum RegisterStatsEnum
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Seasonly = 4,
    Yearly = 5
}
