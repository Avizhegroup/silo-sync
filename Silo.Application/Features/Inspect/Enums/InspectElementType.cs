namespace Silo.Application.Features;

public enum InspectElementType
{
    NotSpecified = 0,
    MultiOption = 1, // radio button or combobox
    OneOption = 2, // checkbox
    Int = 3,
    String = 4
}