namespace Silo.Application.Features;

public class SaveDocumentForGeneralCommand : TeamYarBaseCommand , IRequest<SaveDocumentForGeneralVm>
{
    public List<SaveDocumentForGeneralHeaderDto> List { get; set; }
}
