using System.Text.Json;
using System.Text.Json.Serialization;
using Silo.Application.Dto.Filter;

namespace Silo.Application.Features;
public class GetReportFormatsByPathVm
{
    public int Id { get; set; }
    public ReportFormatTypes Type { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Type), ResourceType = typeof(TextResources))]
    public string TypeString
    {
        get
        {
            return Type switch
            {
                ReportFormatTypes.Column => TextResources.APP_StringKeys_Columns_Data,
                ReportFormatTypes.Filter => TextResources.APP_StringKeys_Filters
            };
        }
    }

    [Display(Name = nameof(TextResources.APP_StringKeys_Name), ResourceType = typeof(TextResources))]
    public string Path { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_Name), ResourceType = typeof(TextResources))]
    public string Name { get; set; }

    [Display(Name = nameof(TextResources.APP_StringKeys_User), ResourceType = typeof(TextResources))]
    public string User { get; set; }

    public string Details { get; set; }
    public List<ReportFormatDetail> DetailsList     
    {
        get => JsonSerializer.Deserialize<List<ReportFormatDetail>>(Details);
    }
}

public class ReportFormatDetail
{
    public string Id { get; set; }
    public string Value { get; set; }
    public ReportFormatDetailTypes DetailType { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
    public ReportColumnSortType SortType { get; set; } = ReportColumnSortType.None;
    public ReportColumnAggregate AggType { get; set; } = ReportColumnAggregate.None;
}

public enum ReportFormatDetailTypes
{
    Data,
    Calculating,
    Pivot,
    Filter,
    DataMiningElements
}

[JsonSerializable(typeof(ApiResponse<List<GetReportFormatsByPathVm>>))]
public partial class GetReportFormatsByPathVmContext : JsonSerializerContext
{

}
