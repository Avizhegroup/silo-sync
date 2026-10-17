using System.Text.RegularExpressions;

namespace Silo.Modules.Register.Pages;

public partial class ParentPackage
{
    private static readonly Regex CodeRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);

    public bool IsLoading { get; set; }
    public string Serial { get; set; } = string.Empty;
    public string PackageCode { get; set; } = string.Empty;
    public string PackageEpc { get; set; } = string.Empty;
    public List<PackageItem> Items { get; set; } = [];

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();
    }

    public async Task OnSerialKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnAddClick();
        }
    }

    public async Task OnPackageCodeKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnLoadPackageClick();
        }
    }

    public async Task OnAddClick()
    {
        string serial = Serial.Trim();

        if (!CodeRegex.IsMatch(serial))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            var rows = (await Api.PostAsync<List<PackageItem>>("SPGetProductInfo"
                , new KeyValuePair<string, object>("tagEpc", serial))).Value;

            if (rows is null || rows.Count == 0)
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
                return;
            }

            foreach (var row in rows.Where(r => Items.All(i => i.ProductSerial != r.ProductSerial)))
            {
                Items.Add(row);
            }

            Serial = string.Empty;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnLoadPackageClick()
    {
        string code = PackageCode.Trim();
        string epc = PackageEpc.Trim();

        if ((code.Length == 0 || code == "0") && epc.Length == 0)
        {
            return;
        }

        if ((code.Length > 0 && !CodeRegex.IsMatch(code)) || (epc.Length > 0 && !CodeRegex.IsMatch(epc)))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            var rows = await GetChildrenAsync(code == "0" ? string.Empty : code, epc);

            Items = rows;

            if (rows.Count > 0)
            {
                PackageCode = rows[0].TagTreeParentSerial;
                PackageEpc = rows[0].TagTreeParentsEpc;
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnSaveClick()
    {
        string epc = PackageEpc.Trim();

        if (epc.Length == 0 || Items.Count == 0)
        {
            return;
        }

        if (!CodeRegex.IsMatch(epc))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            bool ok = (await Api.PostAsync<bool>("SRegisterNextlevelProductUnit"
                , new KeyValuePair<string, object>("productUnitEpc", epc)
                , new KeyValuePair<string, object>("listEpc", Items.Select(i => i.ProductSerial).ToList())
                , new KeyValuePair<string, object>("userToken", UserId))).Value;

            if (ok)
            {
                var rows = await GetChildrenAsync(string.Empty, epc);
                PackageCode = rows.Count > 0 ? rows[0].TagTreeParentSerial : string.Empty;
            }
            else
            {
                PackageCode = "0";
            }

            Notification.Show(ok ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail
                , ok ? "success" : "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnRemoveClick(PackageItem item)
    {
        IsLoading = true;

        try
        {
            bool ok = (await Api.PostAsync<bool>("SRegisterNextlevelProductUnit"
                , new KeyValuePair<string, object>("productUnitEpc", "-1")
                , new KeyValuePair<string, object>("listEpc", new List<string> { item.ProductSerial })
                , new KeyValuePair<string, object>("userToken", UserId))).Value;

            if (ok)
            {
                Items.Remove(item);
            }

            Notification.Show(ok ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail
                , ok ? "success" : "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void OnClearClick()
    {
        Items = [];
        Serial = PackageCode = PackageEpc = string.Empty;
    }

    private string UserId { get; set; } = string.Empty;

    private async Task<List<PackageItem>> GetChildrenAsync(string parentSerial, string parentEpc)
    {
        var rows = (await Api.PostAsync<List<PackageItem>>("SPGetProductListByParentSerial"
            , new KeyValuePair<string, object>("parentSerial", parentSerial)
            , new KeyValuePair<string, object>("parentEpc", parentEpc))).Value;

        return rows ?? [];
    }

    public class PackageItem
    {
        public string TagEpc { get; set; } = string.Empty;
        public string ProductSerial { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductCount { get; set; }
        public string TagTreeParentSerial { get; set; } = string.Empty;
        public string TagTreeParentsEpc { get; set; } = string.Empty;
    }
}
