using System.Text.RegularExpressions;
using Silo.Application.Features;

namespace Silo.Modules.Register.Pages;

public partial class RegistrationDate
{
    private static readonly Regex SerialRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);
    private static readonly Regex DateRegex = new(@"^14\d{2}/(0[1-9]|1[0-2])/(0[1-9]|[12]\d|3[01])$", RegexOptions.Compiled);
    private static readonly Regex TimeRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

    public bool IsLoading { get; set; }
    public string Serial { get; set; } = string.Empty;
    public string NewDate { get; set; } = string.Empty;
    public string NewTime { get; set; } = string.Empty;
    public GetRevokeTagsVm? Product { get; set; }
    public List<GetRevokeTagsVm> ProductList => Product is null ? [] : [Product];
    public bool DateError => NewDate.Length > 0 && !DateRegex.IsMatch(NewDate);
    public bool TimeError => NewTime.Length > 0 && !TimeRegex.IsMatch(NewTime);
    public bool IsInputValid => DateRegex.IsMatch(NewDate) && TimeRegex.IsMatch(NewTime);

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        await Task.CompletedTask;
    }

    public async Task OnSearchClick() => await LoadProductAsync();

    public async Task OnSerialKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await LoadProductAsync();
        }
    }

    public async Task OnSaveClick()
    {
        if (Product is null)
        {
            return;
        }

        if (!DateRegex.IsMatch(NewDate) || !TimeRegex.IsMatch(NewTime))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            bool ok = (await Api.PostAsync<bool>("SChangeRegisterDate"
                , new KeyValuePair<string, object>("ProductSerial", Product.ProductSerial)
                , new KeyValuePair<string, object>("NewDate", NewDate)
                , new KeyValuePair<string, object>("NewTime", NewTime))).Value;

            Notification.Show(ok ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail
                , ok ? "success" : "error");

            if (ok)
            {
                Product = null;
                NewDate = NewTime = string.Empty;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadProductAsync()
    {
        Serial = Serial.Trim();

        if (!SerialRegex.IsMatch(Serial))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            var products = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetProductBySerial"
                , new GetRevokeTagsVmContext()
                , new KeyValuePair<string, object>("serial", Serial))).Value;

            Product = products?.FirstOrDefault();

            if (Product is null)
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }

            Serial = string.Empty;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
