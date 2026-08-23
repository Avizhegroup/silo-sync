using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetPrintsByPrintActionIdDto
{
    public string? ProductSerial { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? ProductTypeTitle { get; set; }
    public decimal? ProductCount { get; set; }
    public decimal? ProductItemCount { get; set; }
    public decimal? ProductCountInPack { get; set; }
    public string? ProductUnit { get; set; }
    public string? ProductSize { get; set; }
    public string? ProductRegCode { get; set; }
    public string? ProductStatusTitle { get; set; }
    public bool RegisterFlag { get; set; }   // CAST AS BIT (non-null because no COALESCE)
    public int? DocumentId { get; set; }
    public string? ProductProperties { get; set; }
    public string? ProductProductionLine { get; set; }
    public string? ProductProductionShift { get; set; }
    public string ProductTypeCode { get; set; } = string.Empty;
    public decimal? ProductPackValue { get; set; }   // COALESCE(...,0)
    public decimal? ProductPackWeight { get; set; }
    public string ProductStatusCode { get; set; } = string.Empty;
    public string DestinationCode { get; set; } = string.Empty;
    public int PrintActionId { get; set; }

    [JsonIgnore]
    public bool IsChoosed { get; set; } = true;
}
