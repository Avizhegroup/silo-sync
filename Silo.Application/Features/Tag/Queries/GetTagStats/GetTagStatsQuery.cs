namespace Silo.Application.Features;

public class GetTagStatsQuery
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string Regcode { get; set; }
    public string Shift { get; set; }
    public string Pl { get; set; }
}