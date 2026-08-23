using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductStoreTransactionsBySerialVm
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ActionType))]
    public string ActionTypeTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_OperationCode))]
    public int ActionId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_From))]
    public string SourceStoreTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Destination))]
    public string DestinationStoreTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Date))]
    public string ActionDate { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Time))]
    public string ActionTime { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_UHF_Log_Id))]
    public string ActionUHFLogId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Description))]
    public string ActionDescription { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_User))]
    public string ActionUserName { get; set; }

}

[JsonSerializable(typeof(ApiResponse<List<GetProductStoreTransactionsBySerialVm>>))]
public partial class GetProductStoreTransactionsBySerialVmContext : JsonSerializerContext
{

}
