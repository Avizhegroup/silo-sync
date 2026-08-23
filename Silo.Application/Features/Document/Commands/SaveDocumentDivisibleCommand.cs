namespace Silo.Application.Features;
public class SaveDocumentDivideCommand
{
    public string CurrentDocKey { get; set; }
    public string CurrentDocType { get; set; }
    public List<GetRemainDividableDocumentItemsVm> RemainDocItems { get; set; } = new();
    public List<GetRemainDividableDocumentItemsVm> NewDivisionDocItems { get; set; } = new();
    public string Description { get; set; }
}
