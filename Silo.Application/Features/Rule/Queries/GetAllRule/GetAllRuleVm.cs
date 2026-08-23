namespace Silo.Application.Features;

public class GetAllRuleVm
{
    public string Id { get; set; } = "0";
    public string Title { get; set; }
    public string Command { get; set; } = string.Empty;
    public string Status { get; set; }
    public string Type { get; set; }
    public string ResultType { get; set; }
    public string StationCode { get; set; }
    public string ReturnResultTrue { get; set; }
    public string ReturnResultFalse { get; set; }
    public string RegUser { get; set; }
    public string RegDate { get; set; }
    public string Username { get; set; }
    public string DefaultTypes { get; set; }
}
