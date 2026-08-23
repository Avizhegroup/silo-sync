using Newtonsoft.Json.Linq;

namespace Silo.Application.Features;
public class AndroidSyncDataDto
{
    public AndroidSyncOpTypes OpType { get; set; }
    public List<JToken> Data { get; set; }
}

public class AndroidSyncDataCommand
{
    public List<AndroidSyncDataDto> Commands { get; set; }
}
