namespace Silo.Api.External.Sharif.Models;

public class SharifErrorResponse
{
    public string TraceId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<SharifErrorDetail> Details { get; set; } = new();
}

public class SharifErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
