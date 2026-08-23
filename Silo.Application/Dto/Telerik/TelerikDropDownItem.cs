namespace Silo.Application.Dto;
public class TelerikDropDownItem
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class TelerikDropDownItemGeneric<T>
{
    public bool IsChoosen { get; set; } = false;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    public string Name { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public T Value { get; set; }
}
