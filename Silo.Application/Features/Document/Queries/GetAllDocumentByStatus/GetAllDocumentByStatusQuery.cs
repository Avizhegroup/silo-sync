namespace Silo.Application.Features;
public class GetAllDocumentByStatusQuery
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Document_Type))]
    public string DocumentType { get; set; }

    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string ProductCode { get; set; }
    public int Status { get; set; }
    public string DocumentKey { get; set; }
    public bool GetCurrentStatusOnly { get; set; } = true;
    public string DocumentHeaderText { get; set; }
    public int? Limit { get; set; }
}
