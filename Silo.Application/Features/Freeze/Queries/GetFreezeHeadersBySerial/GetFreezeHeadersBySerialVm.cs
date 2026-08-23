using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetFreezeHeadersBySerialVm
{
    public int Id { get; set; }

    [StringLength(128)]
    public string? UserId { get; set; }

    public string UserName { get; set; }

    public DateTime? SaveDateTime { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    public bool Status { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetFreezeHeadersBySerialVm>>))]
public partial class GetFreezeHeadersBySerialVmContext : JsonSerializerContext
{

}
