using Newtonsoft.Json;

namespace Silo.Service.Sharif.Dtos;
public class DeviceInfo
{

    private int id;

    [JsonProperty("id")]
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    private string _type;

    [JsonProperty("_type")]
    public string Type
    {
        get { return _type; }
        set { _type = value; }
    }
    private string ip;

    [JsonProperty("ip")]
    public string Ip
    {
        get { return ip; }
        set { ip = value; }
    }
    private int port;

    [JsonProperty("port")]
    public int Port
    {
        get { return port; }
        set { port = value; }
    }

}
