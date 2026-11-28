namespace Silo.Application.Features;
public class SetPrintFormatCommand : TeamYarBaseCommand , IRequest<SetPrintFormatVm>
{
    /// <summary>
    /// سری های ساخت
    /// </summary>
    public List<string> Serials { get; set; }

    /// <summary>
    /// کد قالب چاپ
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string PrintFormatCode { get; set; }
}
