using Silo.Application.Features;

namespace Silo.Pages.Reports;
public partial class ReportNotif
{
    public bool IsLoading = true;
    public GetNotificationQueueQuery Request = new();
    public List<GetAllNotificationQueueVm> Queues;
    public List<GetAllNotificationOrderVm> Orders;
    public List<ChoosableKeyValue> SendTypes;
    public List<ChoosableKeyValue> SendStatus;

    [Inject] public RfidConnectApi Api { get; set; }
    
    public Modal ModalOrders { get; set; }

    protected override async Task SiloInitializer()
    {
        SendTypes = new()
        {
            new()
            {
                IsChoosed = true,
                Key = "EMAIL",
                Value = "0"
            },
            new()
            {
                IsChoosed = false,
                Key = "SMS",
                Value = "1"
            },
            new()
            {
                IsChoosed = false,
                Key = "PUSH",
                Value = "2"
            },
            new()
            {
                IsChoosed = false,
                Key = "WHATSAPP",
                Value = "3"
            },
            new()
            {
                IsChoosed = false,
                Key = "TELEGRAM",
                Value = "4"
            }
        };

        SendStatus = new()
        {
            new()
            {
                Key = TextResources.APP_StringKeys_Notification_Status_WatingSend,
                Value = "0"
            },
            new()
            {
                Key = TextResources.APP_StringKeys_Notification_Status_Sended,
                Value ="1"
            }
        };

        Orders = (await Api.PostAsync<List<GetAllNotificationOrderVm>>("SGetNotification")).Value;

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Queues = null;
    }

    public async Task OnClickSubmit(EditContext e)
    {
        IsLoading = true;

        Queues = (await Api.PostAsyncByUriAndContext<List<GetAllNotificationQueueVm>>("wms/Notification"
            , "SReportNotificationQueue"
            , new GetAllNotificationQueueVmContext()
            , new KeyValuePair<string, object>("search", Request))).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnChooseOrder(GetAllNotificationOrderVm order)
    {
        Request.OrderId = order.Id.ToString();

        Request.OrderTitle = order.Title;

        Request.SendType = order.SendType;

        await ModalOrders.Close(new());
    }
}
