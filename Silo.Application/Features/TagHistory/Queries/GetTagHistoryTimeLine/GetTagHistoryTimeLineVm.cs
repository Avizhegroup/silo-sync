using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetTagHistoryTimeLineVm
{
    public TagEventType TagEventType { get; set; }
    public string TagEventDescription { get; set; }
    public DateTime TagEventDateTime { get; set; }
    public string TagEventDateTimeShamsi { get; set; }
    public string Icon { get
        {
            return TagEventType switch
            {
                TagEventType.None => "",
                TagEventType.Product => "factory",
                TagEventType.Inspect => "manage_search",
                TagEventType.Freeze => "lock",
                TagEventType.Movement => "assistant_navigation",
                TagEventType.Inventory => "warehouse",
                TagEventType.Placement => "share_location",
                TagEventType.Gate => "gate",
                TagEventType.Revoke => "label_off",
                TagEventType.GateAlert => "warning",
                TagEventType.Sell => "storefront",
                TagEventType.Guarantee => "new_releases",
                TagEventType.Expire => "warning"
            };

        }
    }
    public string Color { get
        {
            return TagEventType switch
            {
                TagEventType.None => "",
                TagEventType.Product => "btn-primary",
                TagEventType.Inspect => "btn-secondary",
                TagEventType.Freeze => "btn-warning",
                TagEventType.Movement => "btn-success",
                TagEventType.Inventory => "btn-danger",
                TagEventType.Placement => "btn-info",
                TagEventType.Gate => "btn-light",
                TagEventType.Revoke => "btn-dark",
                TagEventType.GateAlert => "btn-danger",
                TagEventType.Sell => "btn-info",
                TagEventType.Guarantee => "btn-primary",
                TagEventType.Expire => "btn-warning"
            };
        }
    }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagHistoryTimeLineVm>>))]
public partial class GetTagHistoryTimeLineVmContext : JsonSerializerContext
{

}
