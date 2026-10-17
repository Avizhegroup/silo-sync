using System.ComponentModel.DataAnnotations;

namespace Silo.Modules.Register.Pages;

public partial class RfidRegistration
{
    private readonly RegistrationInput Request = new();
    private bool IsLoading;
    private bool Succeeded;
    private string Message = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private async Task OnSubmit()
    {
        Message = string.Empty;
        Succeeded = false;

        Request.Epc = Request.Epc.Trim();
        Request.Serial = Request.Serial.Trim();
        if (Request.Epc.Length != 24)
        {
            Message = TextResources.APP_StringKeys_Validation_BadFormat;
            return;
        }

        IsLoading = true;

        try
        {
            string username = (await AuthState.GetAuthenticationStateAsync()).User.Identity?.Name ?? string.Empty;
            string? existingSerial = (await Api.PostAsync<string>("SCheckTagEPCReturnProductSerial",
                new KeyValuePair<string, object>("TagEpc", Request.Epc))).Value;

            if (!string.IsNullOrWhiteSpace(existingSerial))
            {
                Message = TextResources.APP_StringKeys_Validation_AnyData;
                return;
            }

            int serialStatus = (await Api.PostAsync<int>("SIsProductSerialExist",
                new("serial", Request.Serial),
                new("username", username),
                new("deviceIp", "0"),
                new("deviceId", "0"))).Value;

            if (serialStatus != 1)
            {
                Message = TextResources.APP_StringKeys_Validation_NotFound;
                return;
            }

            bool result = (await Api.PostAsync<bool>("SRegisterTag",
                new("productSerial", Request.Serial.Trim()),
                new("productCode", ""),
                new("productCount", ""),
                new("epcList", new[] { Request.Epc.Trim() }),
                new("username", username),
                new("deviceIp", "0"),
                new("deviceId", "0"),
                new("RegisterProperties", ""))).Value;

            Succeeded = result;
            Message = result ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail;
            if (result)
            {
                Request.Serial = string.Empty;
                Request.Epc = string.Empty;
            }
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

    private sealed class RegistrationInput
    {
        [Required]
        public string Serial { get; set; } = string.Empty;

        [Required]
        public string Epc { get; set; } = string.Empty;
    }
}
