namespace Silo.Application.Features;

public class GetTablesChangeLogDto
{
    public long Id { get; set; }
    public string? TableName { get; set; }
    public string? RecordKey { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}
