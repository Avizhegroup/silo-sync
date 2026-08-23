namespace Silo.Application.Dto.Filter;

public class ReportColumn
{
    public int Id { get; set; }
    public string Title { get; set; }
}

public class ReportColumnGeneric<T> : ReportColumn where T : struct
{
    public T Type { get; set; }
    public string Value { get; set; }
    public bool IsColumnShown { get; set; }
    public ReportColumnSortType SortType { get; set; } = ReportColumnSortType.None;
    public ReportColumnAggregate AggType { get; set; } = ReportColumnAggregate.None;
    public Dictionary<string,string> AdditionalData { get; set; }
}

public class ReportCalculatingColumn<T> : ReportColumn where T : struct
{
    public T GroupColumnType { get; set; }
    public ReportCalculatingColumnType Type { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; }
    public string FieldName { get; set; }
}

public enum ReportCalculatingColumnType
{
    Count,
    Sum,
    Min,
    Max,
    Avg,
    Percent
}

public enum ReportColumnSortType
{
    None,
    Asc,
    Desc
}

public enum ReportColumnAggregate
{
    None,
    Sum,
    Count,
    Avg
}
