namespace Silo.Application.Features;

public class RevokeTagForTeamYarCommand : TeamYarBaseCommand, IRequest<RevokeTagForTeamYarVm>
{
    /// <summary>
    /// لیست سری ساخت
    /// </summary>
    [Required]
    public List<string> Serials { get; set; }

    /// <summary>
    /// کد سند
    /// </summary>
    [Required]
    public string DocumentCode { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Desc { get; set; }
}
