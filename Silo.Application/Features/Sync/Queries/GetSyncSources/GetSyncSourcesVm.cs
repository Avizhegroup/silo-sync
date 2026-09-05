namespace Silo.Application.Features;

public class GetSyncSourcesVm
{
    public int Id { get; set; }
    public string SourceKey { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? SourceType { get; set; }
    public string Command { get; set; } = null!;
    public string FieldKey { get; set; } = null!;
    public string FieldCheck { get; set; } = null!;
    public string FieldOrder { get; set; } = null!;
    public int? IntervalSeconds { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
