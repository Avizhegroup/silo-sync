using MediatR;

namespace Silo.Application.Features;
public class GetPlaceByTruckCrossIdQuery : IRequest<GetPlaceByTruckCrossIdVm>
{
    public int? TruckCrossId { get; set; }
}
