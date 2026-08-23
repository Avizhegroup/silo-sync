namespace Silo.Application.Features;
public class GetAllTagChangeLogQuery: IRequest<GetTablesChangeLogVm>
{
    public string? TableName { get; init; }
    public string? RecordKey { get; init; }
}
