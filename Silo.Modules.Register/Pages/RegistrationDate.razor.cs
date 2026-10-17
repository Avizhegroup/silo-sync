using System.ComponentModel.DataAnnotations;

namespace Silo.Modules.Register.Pages;

public partial class RegistrationDate
{
    private readonly DateUpdateInput Request = new();
    private bool IsLoading;
    private bool Succeeded;
    private string Message = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private async Task OnSubmit()
    {
        Message = string.Empty;
        IsLoading = true;
        try
        {
            Succeeded = (await Api.PostAsync<bool>("SChangeRegisterDate",
                new("ProductSerial", Request.Serial.Trim()),
                new("NewDate", Request.Date),
                new("NewTime", Request.Time))).Value;

            Message = Succeeded ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail;
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

    private sealed class DateUpdateInput
    {
        [Required]
        public string Serial { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^14\d{2}/\d{2}/\d{2}$")]
        public string Date { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{2}:\d{2}$")]
        public string Time { get; set; } = string.Empty;
    }
}
