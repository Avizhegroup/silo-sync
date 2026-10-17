namespace Silo.Modules.Register.Pages;

public partial class GroupVerification
{
    private string EpcInput = string.Empty;
    private string Message = string.Empty;
    private bool IsLoading;
    private List<string> Epcs = new();
    private List<TagVerificationRow>? Results;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private void AddEpc()
    {
        string epc = EpcInput.Trim();
        if (epc.Length == 24 && !Epcs.Contains(epc, StringComparer.OrdinalIgnoreCase))
        {
            Epcs.Add(epc);
            EpcInput = string.Empty;
            Message = string.Empty;
        }
        else
        {
            Message = TextResources.APP_StringKeys_Validation_BadFormat;
        }
    }

    private void OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            AddEpc();
        }
    }

    private async Task Verify()
    {
        if (Epcs.Count == 0)
        {
            Message = TextResources.APP_StringKeys_Required;
            return;
        }

        IsLoading = true;
        Message = string.Empty;
        try
        {
            Results = (await Api.PostAsync<List<TagVerificationRow>>("SGetProductsByEpcs",
                new("epcList", Epcs.ToArray()),
                new("userToken", (await AuthState.GetAuthenticationStateAsync()).User.GetUserId()))).Value ?? new();
        }
        catch
        {
            Message = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Clear()
    {
        EpcInput = string.Empty;
        Epcs.Clear();
        Results = null;
        Message = string.Empty;
    }

    private sealed class TagVerificationRow
    {
        public string TagEpc { get; set; } = string.Empty;
        public string ProductSerial { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string RegCode { get; set; } = string.Empty;
        public string ContractStatus { get; set; } = string.Empty;
    }
}
