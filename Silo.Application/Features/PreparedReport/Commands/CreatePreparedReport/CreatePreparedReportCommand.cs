namespace Silo.Application.Features;
public class CreatePreparedReportCommand : IRequest<CreatePreparedReportVm>
{
    [Required]
    public string Title { get; set; }
    public List<KeyValuePair<string, object>> Variables { get; set; } = new();
    public List<KeyValuePair<string, object>> DataSources { get; set; } = new();
    public List<KeyValuePair<string, string>> Images { get; set; } = new();

    [Required]
    public string ReportFileName { get; set; }
}
