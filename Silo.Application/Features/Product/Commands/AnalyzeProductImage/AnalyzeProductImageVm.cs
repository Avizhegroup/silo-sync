namespace Silo.Application.Features;

public class AnalyzeProductImageVm
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }

    /// <summary>Normalized pattern JSON, ready to be stored in tbl_Products.fld_ProductPattern.</summary>
    public string Pattern { get; set; }
    public string Summary { get; set; }
    public List<string> Keywords { get; set; } = [];
}
