namespace Silo.Application.Features;

public class VerifyLocateForTeamYarCommand : TeamYarBaseCommand, IRequest<VerifyLocateForTeamYarVm>
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
    /// کد انبار
    /// </summary>
    [Required]
    public string WarehouseCode { get; set; }

    /// <summary>
    /// کد شناسه عملیات در سامانه RFID
    /// </summary>
    [Required]
    public string OperationCode { get; set; }
}
