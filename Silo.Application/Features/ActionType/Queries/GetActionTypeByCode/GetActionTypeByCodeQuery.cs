using Silo.Application.Features;

public class GetActionTypeByCodeQuery : IRequest<GetActionTypeByCodeVm>
{
    public int Code { get; set; }
}
