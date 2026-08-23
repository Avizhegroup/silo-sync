using Silo.Application.Features;

namespace Silo.Application.Dto.DynamicField;
public class DynamicFieldWithValueDto
{
    public string Title { get; set; }
    public string Value { get; set; }
    public string EffectiveValue
    {
        get => string.IsNullOrEmpty(Value) ? DefaultValue : Value;
        set => Value = value;
    }
    public double? NumericValue
    {
        get => double.TryParse(EffectiveValue, out var v) ? v : null;
        set => Value = value?.ToString();
    }
    public DynamicFieldValueType ValueType { get; set; }
    public List<string> ValueOptions { get; set; }
    public string DefaultValue { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public int Order { get; set; }
}
