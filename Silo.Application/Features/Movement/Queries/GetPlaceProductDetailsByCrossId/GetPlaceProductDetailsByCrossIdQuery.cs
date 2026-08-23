using MediatR;

namespace Silo.Application.Features;
public class GetPlaceProductDetailsByCrossIdQuery : IRequest<GetPlaceProductDetailsByCrossIdVm>
{
    public int TruckCrossId { get; set; }
    public string ProductCode { get; set; }
    public string? DocumentCode { get; set; }
}
