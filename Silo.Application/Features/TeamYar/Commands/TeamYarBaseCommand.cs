namespace Silo.Application.Features;

public class TeamYarBaseCommand
{
    /// <summary>
    /// کد یکتای درخواست برای جلوگیری از ثبت چندباره یک درخواست
    /// </summary>
    [Required]
    public string CommandUniqueId { get; set; }
}
