namespace Silo.Application.Dto;

public class ChoosableKeyValue
{
    public string Key { get; set; }
    public string Value { get; set; }
    public bool IsChoosed { get; set; } = false;
    public bool IsEditable { get; set; } = true;
}
