using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllShiftsVm
{
  
        [JsonPropertyName("fld_ProductPropertyBId")]
        public string Code { get; set; }

        [JsonPropertyName("fld_ProductPropertyBTitle")]
        public string Title { get; set; }

    
}

[JsonSerializable(typeof(ApiResponse<List<GetAllShiftsVm>>))]
public partial class GetShiftObjectContext : JsonSerializerContext
{

}
