namespace Silo.Modules.Register.Pages;

public partial class RegisterDataReport
{
    private string Serial = string.Empty;
    private string FromDate = string.Empty;
    private string ToDate = string.Empty;
    private string Message = string.Empty;
    private bool IsLoading;
    private List<RegisterReportRow>? Rows;

    [Parameter] public string ReportType { get; set; } = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private string ReportTitle => ReportType == "cancelled"
        ? TextResources.APP_StringKeys_Revoke
        : TextResources.APP_StringKeys_Validation_BadFormat;

    protected override async Task OnParametersSetAsync()
    {
        Rows = null;
        Message = string.Empty;
    }

    private async Task Search()
    {
        if (ReportType is not ("cancelled" or "errors"))
        {
            return;
        }

        IsLoading = true;
        Message = string.Empty;
        try
        {
            if (ReportType == "cancelled")
            {
                Rows = (await Api.PostAsync<List<RegisterReportRow>>("SRepSCancelRegisterTag",
                    new("ProductSerial", string.IsNullOrWhiteSpace(Serial) ? "-1" : Serial.Trim()),
                    new("FromDate", string.IsNullOrWhiteSpace(FromDate) ? "-1" : FromDate.Replace("/", string.Empty, StringComparison.Ordinal)),
                    new("ToDate", string.IsNullOrWhiteSpace(ToDate) ? "-1" : ToDate.Replace("/", string.Empty, StringComparison.Ordinal)),
                    new("User", "-1"))).Value;
            }
            else
            {
                Rows = (await Api.PostAsync<List<RegisterReportRow>>("SRepRegisterTagWithOutEPC")).Value;
            }

            private sealed class RegisterReportRow
            {
                public string ProductSerial { get; set; } = string.Empty;
                public string ProductCode { get; set; } = string.Empty;
                public string ProductName { get; set; } = string.Empty;
                public string RegCode { get; set; } = string.Empty;
                public decimal ProductCount { get; set; }
                public string Date => !string.IsNullOrWhiteSpace(SoftDeleteDate)
                    ? SoftDeleteDate
                    : !string.IsNullOrWhiteSpace(TagRegisterShamsiUnixDate) ? TagRegisterShamsiUnixDate : RegisterDate;
                public string User => !string.IsNullOrWhiteSpace(SoftDeleteUser) ? SoftDeleteUser : Username;
                public string RegisterDate { get; set; } = string.Empty;
                public string SoftDeleteDate { get; set; } = string.Empty;
                public string SoftDeleteUser { get; set; } = string.Empty;
                public string Username { get; set; } = string.Empty;
                public string TagRegisterShamsiUnixDate { get; set; } = string.Empty;
            }
        }
        catch
        {
            Rows = null;
            Message = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
