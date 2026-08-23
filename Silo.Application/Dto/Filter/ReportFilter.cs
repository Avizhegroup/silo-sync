
namespace Silo.Application.Dto;
public class ReportFilter
{
    public int Id { get; set; }
    public string FieldName { get; set; }
    public string Value { get; set; }
    public decimal NumericValue 
    { 
        get 
        {
            if (decimal.TryParse(Value, out decimal parsedValue))
            {
                return parsedValue;
            }

            return 0;
        }
        set
        {
            Value = value.ToString();
        }
    }
    public List<string> Values { get; set; }
    public string Label { get; set; }
    public List<ReportDataItem> Items { get; set; } = new();
    public FilterComponent Component { get; set; } = FilterComponent.NotSpecified;
    public FilterType Type { get; set; } = FilterType.Static;
    public FilterAddType AddType { get; set; } = FilterAddType.And;
    public FilterEqualityType EqualityType { get; set; } = FilterEqualityType.Equals;
    public bool IsLike { get; set; } = false;
    public bool IsEditable { get; set; } = true;
    public bool IsLikeCheckboxShown { get; set; }
    public string SqlWhereCommand { get; set; }
    public int FilterId { get; set; }
    public bool IsFilterShown { get; set; }
    public Dictionary<string,string> AdditionalData { get; set; }
}

public class ReportFilterGeneric<T> : ReportFilter  where T : struct
{
    public T FieldType { get; set; }
}

public enum FilterComponent
{
    NotSpecified,
    Drop,
    Text,
    PersianDate,
    Modal,
    ProductCodeModal,
    LocationModal,
    Time,
    RichTextEditor,
    Numeric
}

public enum FilterType
{
    Static,
    Dynamic,
    TechnicalInfo,
    InspectElement,
    DataMiningElement,
    Limit
}

public enum FilterAddType
{
    And,
    Or
}

public enum FilterEqualityType
{
    Equals,
    BiggerThan,
    SmallerThan,
    Like
}

