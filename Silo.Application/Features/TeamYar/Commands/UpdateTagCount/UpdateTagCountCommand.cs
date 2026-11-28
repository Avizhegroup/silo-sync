namespace Silo.Application.Features;
public class UpdateTagCountCommand : TeamYarBaseCommand, IRequest<UpdateTagCountVm>
{
    public List<UpdateTagCountDto> List  { get; set; }
}
