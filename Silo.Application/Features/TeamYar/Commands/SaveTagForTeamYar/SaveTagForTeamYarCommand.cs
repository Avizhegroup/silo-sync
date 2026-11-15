namespace Silo.Application.Features;
public class SaveTagForTeamYarCommand : TeamYarBaseCommand , IRequest<SaveTagForTeamYarVm>
{
    public List<SaveTagForTeamYarDto> Data { get; set; }
}
