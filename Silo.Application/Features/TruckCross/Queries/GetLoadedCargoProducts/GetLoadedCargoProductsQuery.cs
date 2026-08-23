using MediatR;

namespace Silo.Application.Features;
public class GetLoadedCargoProductsQuery : IRequest<GetLoadedCargoProductsVm>
{
    public int? TruckCrossId { get; set; }
}
