namespace Silo.Application.Dto;

public class TelerikContextMenuItem
{
    public string Text { get; set; }
    public bool Disabled { get; set; }
    public bool Separator { get; set; }
    public string Icon { get; set; }
    public List<TelerikContextMenuItem> Items { get; set; }
    public Action Action { get; set; }
}
