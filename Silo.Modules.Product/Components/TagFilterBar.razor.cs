using Silo.Shared.Components.Modals;

namespace Silo.Modules.Product.Components;
public partial class TagFilterBar
{
    public string NewProductSerial;

    public ProductSerialModal ProductModal { get; set; }
    public BarcodeModal BarcodeModal { get; set; }

    [Parameter] public EventCallback<string> OnSerialSelected { get; set; }
    [Parameter] public EventCallback OnClear { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    [CascadingParameter] public TelerikNotification FilterBarNotification { get; set; }

    public async Task OnSerialKeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await OnSearchClick(new());
        }
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        if (NewProductSerial.HasNoValue())
        {
            FilterBarNotification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_ProductSerial), "error");
         
            return;
        }

        await OnSerialSelected.InvokeAsync(NewProductSerial);
    }

    public async Task OnSerialsSelected(List<GetAllProductBySerialVm> products)
    {
        NewProductSerial = products[0].ProductSerial;
    
        await OnSerialSelected.InvokeAsync(NewProductSerial);
    }

    public async Task OnBarcodeSelected(string barcode)
    {
        if (barcode.HasValue())
        {
            NewProductSerial = barcode;
           
            await OnSerialSelected.InvokeAsync(barcode);
        }
    }

    public async Task OnLastReadedByHandHeldClick(MouseEventArgs e)
    {
        var result = (await Api.PostAsync<string>("SGetLastReadedTagForHistory")).Value;

        if (result.HasValue())
        {
            NewProductSerial = result;
        
            await OnSerialSelected.InvokeAsync(result);
        }
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        NewProductSerial = string.Empty;
       
        await OnClear.InvokeAsync();
    }
}
