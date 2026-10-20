namespace Silo.Modules.Register.Models;

/// <summary>The state columns of <c>SPGetProductInfo</c> needed to decide whether a registration date may be edited.</summary>
public sealed class TagStateInfo
{
    /// <summary>The server title for a tag whose last inspection result is empty and that is not locked.</summary>
    public const string NotInspectedTitle = "بازرسی نشده";

    /// <summary>tbl_Tags.TagStatus value of a tag that is still in the production warehouses.</summary>
    public const string ProductionStatus = "0";

    public string TagEpc { get; set; } = string.Empty;
    public string ProductSerial { get; set; } = string.Empty;
    public string TagStatus { get; set; } = string.Empty;
    public string TagStatusTitle { get; set; } = string.Empty;
    public string InspectStatusTitle { get; set; } = string.Empty;

    /// <summary>
    /// Legacy rule (F10): the registration date can only be changed while the product has not been inspected and is
    /// still in the production warehouses. The inspect state exists on the server only as a Persian title, so it is
    /// compared by that title; the warehouse state uses the numeric status.
    /// </summary>
    public static (bool CanEdit, string Reason) EvaluateDateEdit(TagStateInfo? state)
    {
        if (state is null)
        {
            return (false, TextResources.APP_StringKeys_Tag_Not_Register);
        }

        var notInspected = string.Equals(state.InspectStatusTitle?.Trim(), NotInspectedTitle, StringComparison.Ordinal);
        var inProduction = string.Equals(state.TagStatus?.Trim(), ProductionStatus, StringComparison.Ordinal);

        return notInspected && inProduction
            ? (true, "The registration date can be changed.")
            : (false, "The product has been inspected or has left the production warehouses, so its registration date cannot be changed.");
    }
}
