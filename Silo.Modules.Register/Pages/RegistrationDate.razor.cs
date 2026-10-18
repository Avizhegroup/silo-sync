using System.Text.RegularExpressions;
using Silo.Application.Features;
using Silo.Modules.Register.Models;

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
    public bool CanEdit { get; set; }
    public string EditReason { get; set; } = string.Empty;
    public List<GetRevokeTagsVm> ProductList => Product is null ? [] : [Product];
    public bool DateError => NewDate.Length > 0 && !DateRegex.IsMatch(NewDate);
    public bool TimeError => NewTime.Length > 0 && !TimeRegex.IsMatch(NewTime);
    public bool IsInputValid => CanEdit && DateRegex.IsMatch(NewDate) && TimeRegex.IsMatch(NewTime);

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
            // The server does not enforce the rule, so check it again right before writing.
            var current = await GetTagStateAsync(Product.ProductSerial);
            (CanEdit, EditReason) = TagStateInfo.EvaluateDateEdit(current);
            if (!CanEdit)
            {
                Notification.Show(EditReason, "error");
                return;
            }

            bool ok = (await Api.PostAsync<bool>("SChangeRegisterDate"
                , new KeyValuePair<string, object>("ProductSerial", Product.ProductSerial)
                , new KeyValuePair<string, object>("NewDate", NewDate)
                , new KeyValuePair<string, object>("NewTime", NewTime))).Value;

            Notification.Show(ok ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail
                , ok ? "success" : "error");

            if (ok)
            {
                Product = null;
                CanEdit = false;
                EditReason = string.Empty;
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
            // The input may be a serial or a tag EPC: SPGetProductInfo resolves either.
            var state = await GetTagStateAsync(Serial);
            (CanEdit, EditReason) = TagStateInfo.EvaluateDateEdit(state);

            Product = null;
            if (state is not null)
            {
                var products = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetProductBySerial"
                    , new GetRevokeTagsVmContext()
                    , new KeyValuePair<string, object>("serial", state.ProductSerial))).Value;

                Product = products?.FirstOrDefault();
            }

            if (Product is null)
            {
                CanEdit = false;
                Notification.Show(state is null ? EditReason : TextResources.APP_StringKeys_Alert_Fail, "error");
            }

            Serial = string.Empty;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<TagStateInfo?> GetTagStateAsync(string serialOrEpc)
        => (await Api.PostAsync<List<TagStateInfo>>("SPGetProductInfo"
            , new KeyValuePair<string, object>("tagEpc", serialOrEpc))).Value?.FirstOrDefault();
}
