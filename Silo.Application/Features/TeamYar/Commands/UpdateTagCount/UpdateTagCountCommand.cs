namespace Silo.Application.Features;

public class UpdateTagCountCommand : IRequest<UpdateTagCountVm>
{
    public List<UpdateTagCountDto> List  { get; set; }
}
