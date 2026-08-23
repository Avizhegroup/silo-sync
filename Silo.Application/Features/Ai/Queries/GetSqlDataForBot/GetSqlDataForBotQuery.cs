namespace Silo.Application.Features;

public class GetSqlDataForBotQuery : IRequest<GetSqlDataForBotVm>
{
    public string Command { get; set; }
}
