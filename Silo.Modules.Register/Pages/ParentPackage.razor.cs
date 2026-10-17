namespace Silo.Modules.Register.Pages;

public partial class ParentPackage
{
    private string ParentEpc = string.Empty;
    private string SerialInput = string.Empty;
    private string Message = string.Empty;
    private bool IsLoading;
    private bool Succeeded;
    private List<SerialRow> Serials = new();

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private void AddSerial()
    {
        string serial = SerialInput.Trim();
        if (serial.Length == 0 || Serials.Any(row => row.Serial.Equals(serial, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        Serials.Add(new SerialRow(serial));
        SerialInput = string.Empty;
        Message = string.Empty;
    }

    private void OnSerialKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            AddSerial();
        }
    }

    private void RemoveSerial(SerialRow row) => Serials.Remove(row);

    private async Task SavePackage()
    {
        Message = string.Empty;
        if (ParentEpc.Trim().Length != 24 || Serials.Count == 0)
        {
            Message = TextResources.APP_StringKeys_Required;
            Succeeded = false;
            return;
        }

        IsLoading = true;
        try
        {
            Succeeded = (await Api.PostAsync<bool>("SRegisterNextlevelProductUnit",
                new("productUnitEpc", ParentEpc.Trim()),
                new("listEpc", Serials.Select(row => row.Serial).ToArray()),
                new("userToken", (await AuthState.GetAuthenticationStateAsync()).User.GetUserId()))).Value;
            Message = Succeeded ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail;
            if (Succeeded)
            {
                string? packageId = (await Api.PostAsync<string>("GetLatestNextlevelProductUnitbyEpc",
                    new KeyValuePair<string, object>("EpcCode", ParentEpc.Trim()))).Value;
                if (!string.IsNullOrWhiteSpace(packageId))
                {
                    Message = $"{TextResources.APP_StringKeys_Alert_Success} {packageId}";
                }
            }
        }
        catch
        {
            Succeeded = false;
            Message = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Clear()
    {
        ParentEpc = string.Empty;
        SerialInput = string.Empty;
        Serials.Clear();
        Message = string.Empty;
        Succeeded = false;
    }

    private sealed record SerialRow(string Serial);
}
