namespace Silo.Modules.Register.Models;

/// <summary>The columns of <c>SGetProductsByEpcs</c> shown by the group test.</summary>
public sealed class GroupTagInfo
{
    public string TagEpc { get; set; } = string.Empty;
    public string ProductSerial { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string RegCode { get; set; } = string.Empty;
    public string ContractStatus { get; set; } = string.Empty;
}

/// <summary>One line of the group test: a tag that was read, registered or not.</summary>
public sealed record GroupTagRow(int Index, string Epc, bool IsRegistered, string Serial, string ProductCode, string ProductName, string RegCode, string ContractStatus);
