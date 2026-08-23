using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class CreatePreparedReportVm
{
    public int Result { get; set; }
}

[JsonSerializable(typeof(ApiResponse<CreatePreparedReportVm>))]
public partial class CreatePreparedReportVmContext : JsonSerializerContext
{
}
