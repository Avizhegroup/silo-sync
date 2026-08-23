using MediatR;

namespace Silo.Application.Features;
public class TransferProductCommand : IRequest<TransferProductVm>
{
    public List<TransferProductDto> NewProducts { get; set; }
}
