namespace Silo.Domains.Android;

[Table("tbl_InspectElements")]
public class InspectElement
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; }
    public InspectElementType InspectElementType { get; set; }
    public string Value { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public bool Prevent { get; set; }
    public bool IsActive { get; set; }
    public bool IsRequired { get; set; }
    public string ProductTypes { get; set; }
    public string Options { get; set; }
    public int RowIdentifier { get; set; }
}

public enum InspectElementType
{
    NotSpecified = 0,
    MultiOption = 1, // radio button or combobox
    OneOption = 2, // checkbox
    Int = 3,
    String = 4
}
