using Silo.Application.Features;

namespace Silo.Application.Dto;
public class InspectElementDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public InspectElementType InspectElementType { get; set; }
    public string Value { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public bool Prevent { get; set; }
    public bool IsActive { get; set; }
    public bool IsRequired { get; set; }
    public string[] ProductTypes { get; set; }
    public string[] Options { get; set; }
    public int Row { get; set; }
}

public class InspectDto
{
    public int Id { get; set; }
    public DateTime SaveDateTime { get; set; }
    public string SaveDate { get; set; }
    public string Serial { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public InspectResult InspectResult { get; set; }
    public List<InspectElementValues> InspectElementResults { get; set; }
}

public class InspectSaveDto
{
    public string Serial { get; set; }
    public string UserId { get; set; }
    public InspectResult InspectResult { get; set; }
    public List<InspectElementValues> InspectElementResults { get; set; }
}

public class InspectReportRequest
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string ProductCode { get; set; }
    public string UserId { get; set; }
    public string Line { get; set; }
    public string RegCode { get; set; }
    public string ProductSerial { get; set; } = "-1";
    public int InspectResult { get; set; }
    public List<InspectElementValues> ElementFilters { get; set; }
    public List<ChoosableKeyValue> DynamicFilters { get; set; } = new();
}

public class InspectReportResponse
{
    public int InspectId { get; set; }
    public string InspectUsername { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
    public string Line { get; set; }
    public DateTime ProductionDateTime { get; set; }
    public DateTime DateTime { get; set; }
    public string RegCode { get; set; }
    public InspectResult Result { get; set; }
    public List<InspectElementValues> Values { get; set; }
    public string ProductProperties { get; set; }
}

public class InspectProductReportQuery
{
    public string ProductCode { get; set; }

    public string FromDate { get; set; }

    public string ToDate { get; set; }

    public string FromTime { get; set; }

    public string ToTime { get; set; }

    public string Shift { get; set; } = "-1";

    public string Line { get; set; } = "-1";

    public string TechnicalCode { get; set; } = "-1";

    public bool TechnicalCodeLike { get; set; } = false;

    public string Qc { get; set; } = "-1";

    public string Size { get; set; } = "-1";
}

public class InspectProduct
{
    public string ProductSerial { get; set; }
    public string RegCode { get; set; }
    public string Size { get; set; }
    public string Line { get; set; }
    public bool Lock { get; set; }
    public string Qc { get; set; }
    public decimal Count { get; set; }
    public long Row { get; set; }
}

public class InspectProductHeader
{
    public string RegCode { get; set; }
    public string Size { get; set; }
    public string Line { get; set; }
    public decimal AcceptedSum { get; set; } = 0;
    public decimal AcceptedCount { get; set; } = 0;
    public decimal RejectedCount { get; set; } = 0;
    public decimal RejectedSum { get; set; } = 0;
    public List<InspectProductItem> Items { get; set; } = new();
}

public class InspectProductItem
{
    public string Qc { get; set; }
    public decimal SumCount { get; set; } = 0;
}

