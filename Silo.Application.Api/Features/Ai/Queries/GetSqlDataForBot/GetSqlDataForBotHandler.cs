using Silo.Application.Contracts;

namespace Silo.Application.Api.Features;

public class GetSqlDataForBotHandler(IDataAccess dataAccess) : IRequestHandler<GetSqlDataForBotQuery, GetSqlDataForBotVm>
{
    public async Task<GetSqlDataForBotVm> Handle(GetSqlDataForBotQuery request, CancellationToken cancellationToken)
    => new()
    {
        Data = DataTableTools.DataTableToObjects(dataAccess.SqlDataAdapter(request.Command))
    };
}
