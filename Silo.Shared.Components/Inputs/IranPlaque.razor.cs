using System.Text.RegularExpressions;

namespace Silo.Shared.Components;

public partial class IranPlaque
{
    private string _lastKnownValue;
    private string _firstPart;
    private string _character;
    private string _secondPart;
    private string _cityPart;

    public readonly List<string> IranianAlphabets = IranianPlaqueTools.GetIranianAlphabets();

    public string FirstPart
    {
        get => _firstPart;
        set 
        {
            _firstPart = value;
            
            Compose(); 
        }
    }

    public string Character
    {
        get => _character;
        set 
        {
            _character = value;
          
            Compose();
        }
    }

    public string SecondPart
    {
        get => _secondPart;
        set 
        {
            _secondPart = value;
           
            Compose(); 
        }
    }

    public string CityPart
    {
        get => _cityPart;
        set 
        {
            _cityPart = value;
        
            Compose();
        }
    }

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public int TabIndex { get; set; }

    protected override void OnParametersSet()
    {
        if (Value != _lastKnownValue)
        {
            ParseValue(Value);
            _lastKnownValue = Value;
        }
    }

    private void ParseValue(string value)
    {
        if (value.HasNoValue())
        {
            _firstPart = _character = _secondPart = _cityPart = null;
           
            return;
        }

        var match = PlaqueRegex().Match(value);
       
        if (match.Success)
        {
            _firstPart = match.Groups[1].Value;

            _character = match.Groups[2].Value;

            _secondPart = match.Groups[3].Value;

            _cityPart = match.Groups[4].Value;
        }
    }

   

    private void Compose()
    {
        Value = $"{_firstPart}{_character}{_secondPart}-{_cityPart}";
        
        _lastKnownValue = Value;

        ValueChanged.InvokeAsync(Value);
    }

    [GeneratedRegex(@"^(\d{2})([^\d-]*)(\d{3})-(\d{2})$")]
    private static partial Regex PlaqueRegex();
}
