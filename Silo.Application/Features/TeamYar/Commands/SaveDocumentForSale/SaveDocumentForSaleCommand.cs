namespace Silo.Application.Features; 

public class SaveDocumentForSaleCommand : TeamYarBaseCommand, IRequest<SaveDocumentForSaleVm>
{
    public List<SaveDocumentForSaleHeaderDto> Documents { get; set; }
}
