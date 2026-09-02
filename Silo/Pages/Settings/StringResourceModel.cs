namespace Silo.Pages.Settings;

public class StringResourceModel
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string? Value { get; set; }

    public bool IsNew { get; set; }

    public bool IsDeleted { get; set; }
}
