using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetDivisionSuggestItemsVm
{
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public string ProductUnit { get; set; }
    public decimal Count { get; set; }

}
[JsonSerializable(typeof(ApiResponse<List<GetDivisionSuggestItemsVm>>))]
public partial class GetDivisionSuggestItemsVmContext : JsonSerializerContext
{
}
