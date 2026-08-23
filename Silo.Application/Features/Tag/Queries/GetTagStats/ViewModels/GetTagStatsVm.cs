using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class OnBrief
{
    public decimal Today { get; set; }
    public decimal Monthly { get; set; }
    public decimal Yearly { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<OnBrief>>))]
public partial class GetOnBriefContext : JsonSerializerContext
{
}
