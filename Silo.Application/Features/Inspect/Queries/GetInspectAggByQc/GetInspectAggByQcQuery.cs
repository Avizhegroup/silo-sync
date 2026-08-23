namespace Silo.Application.Features;

public class GetInspectAggByQcQuery
{
    public string ProductCode { get; set; }

    public string FromDate { get; set; }

    public string ToDate { get; set; }

    public string FromTime { get; set; }

    public string ToTime { get; set; }

    public string Shift { get; set; } = "-1";

    public string Line { get; set; } = "-1";

    public string TechnicalCode { get; set; }

    public bool TechnicalCodeLike { get; set; } = false;

    public string Qc { get; set; } = "-1";

    public string Size { get; set; } = "-1";
}