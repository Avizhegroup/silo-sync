namespace Silo.Application.Features;

public class VerifyLocateForTeamYarCommand : TeamYarBaseCommand, IRequest<VerifyLocateForTeamYarVm>
{
    /// <summary>
    /// لیست سری ساخت
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public List<string> Serials { get; set; }

    /// <summary>
    /// کد سند
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DocumentCode { get; set; }

    /// <summary>
    /// کد انبار
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string WarehouseCode { get; set; }

    /// <summary>
    /// کد شناسه عملیات در سامانه RFID
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string OperationCode { get; set; }
}
