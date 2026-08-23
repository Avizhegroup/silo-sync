using MediatR;

namespace Silo.Application.Features;
public class TransferDocumentCommand : IRequest<TransferDocumentVm>
{
    public TransferDocumentHeaderDto DocumentHeader { get; set; }

    public List<TransferDocumentItemDto> DocumentItems { get; set; }
}
