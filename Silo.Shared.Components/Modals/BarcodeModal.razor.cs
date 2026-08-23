namespace Silo.Shared.Components.Modals;
public partial class BarcodeModal
{
    public bool IsLoading = false;
    public string Barcode = string.Empty;

    [Parameter] public EventCallback<string> OnBarcodeClick { get; set; }

    public Modal Modal { get; set; }
    public ElementReference RefBarcode { get; set; }

    public async Task OnAddClick(MouseEventArgs e)
    {
        IsLoading = true;

        await OnBarcodeClick.InvokeAsync(Barcode);

        Barcode = string.Empty;

        await RefBarcode.FocusAsync();
        
        IsLoading = false;
    }

    public async Task OnBarcodeKeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await OnAddClick(new());
        }
    }

    public async Task Show()
    {
        IsLoading = true;

        Barcode = string.Empty;

        await Modal.Open(new());

        IsLoading = false;
    }
}
