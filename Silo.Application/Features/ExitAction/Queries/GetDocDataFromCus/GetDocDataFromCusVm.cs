using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetDocDataFromCusVm
{
    public string Error { get; set; }
    public string TagEpc { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Regcode { get; set; } = "";
    public string Qc { get; set; } = "";
    public decimal ProductCount { get; set; }
    public decimal SumCount { get; set; }
    public decimal SumValue { get; set; }
    public string Date { get; set; }

    public string StoreCode { get; set; }

    public string StoreName { get; set; }

    public string MaxDate { get; set; }
    public string CountProduct { get; set; }
    public string ProductPackValue { get; set; }
    public string ProductTechnicalCode { get; set; }
    public string Location { get; set; }
    public string SerialList { get; set; } = string.Empty;
    public string TruckCode { get; set; } = string.Empty;
    public bool IsChoosed { get; set; } = false;
    public string DockCode { get; set; } = string.Empty;

    public string Desc { get; set; }
    public string Status { get; set; }

}

[JsonSerializable(typeof(ApiResponse<List<GetDocDataFromCusVm>>))]
public partial class GetDocDataFromCusVmContext : JsonSerializerContext
{
}
