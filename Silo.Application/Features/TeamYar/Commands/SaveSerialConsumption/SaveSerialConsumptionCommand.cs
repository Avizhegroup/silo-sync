namespace Silo.Application.Features;

public class SaveSerialConsumptionCommand : TeamYarBaseCommand, IRequest<SaveSerialConsumptionVm>
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
    /// تاریخ
    /// </summary>
    public string? Date { get; set; }
}
