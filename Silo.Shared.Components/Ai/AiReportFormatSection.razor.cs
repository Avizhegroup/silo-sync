using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Ai;

public partial class AiReportFormatSection
{
    public List<GetReportFormatsByPathVm> ReportFormats { get; set; } = new();
    public GetReportFormatsByPathVm DeleteFormat { get; set; } = new();
    public SaveAiReportCommand FormatCommand { get; set; } = new();
    public bool IsSaving { get; set; }

    [Parameter] public string Mode { get; set; } = string.Empty;
    [Parameter] public int? QueryReferenceId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    [CascadingParameter] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ReloadFormats();
    }

    public async Task OnDeleteFormatClick(GetReportFormatsByPathVm format)
    {
        DeleteFormat = format;
    }

    public async Task OnDeleteFormatConfirmClick(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>(
            "wms/ReportFormat",
            "SDeleteAiReport",
            new KeyValuePair<string, object>("id", DeleteFormat.Id))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
            await ReloadFormats();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            IsLoading = false;
        }

        DeleteFormat = new();
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (string.IsNullOrWhiteSpace(FormatCommand.ReportName))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");
            return;
        }

        if (QueryReferenceId is null or <= 0)
        {
            Notification.Show("شناسه کوئری معتبر نیست", "error");
            return;
        }

        IsSaving = true;
        IsLoading = true;

        try
        {
            FormatCommand.Mode = Mode;
            FormatCommand.QueryId = QueryReferenceId.Value;

            var saveResult = (await Api.PostAsyncByUri<int>(
                "wms/ReportFormat",
                "SSaveAiReport",
                new KeyValuePair<string, object>("command", FormatCommand))).Value;

            if (saveResult > 0)
            {
                // لینک اولیه بدون کاربر (کاربر بعداً از صفحه دسترسی اضافه می‌شود)
                var linkResult = (await Api.PostAsyncByUri<bool>(
                    "wms/ReportFormat",
                    "SSaveAiReportLink",
                    new KeyValuePair<string, object>("reportFormatId", saveResult),
                    new KeyValuePair<string, object>("reportName", FormatCommand.ReportName),
                    new KeyValuePair<string, object>("userIds", new List<string>()))).Value;

                Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
                FormatCommand = new();
                await ReloadFormats();
                await OnSaved.InvokeAsync();
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
        }
        catch (Exception ex)
        {
            Notification.Show($"خطا: {ex.Message}", "error");
        }
        finally
        {
            IsSaving = false;
            IsLoading = false;
        }
    }

    public Task OnInvalidSubmit(EditContext context)
    {
        foreach (var message in context.GetValidationMessages())
            Notification.Show(message, "error");

        return Task.CompletedTask;
    }

    private async Task ReloadFormats()
    {
        IsLoading = true;

        ReportFormats = (await Api.PostAsyncByUriAndContext<List<GetReportFormatsByPathVm>>(
            "wms/ReportFormat",
            "SGetAiReportFormats",
            new GetReportFormatsByPathVmContext()
        )).Value ?? new();

        IsLoading = false;
    }
}

public class SaveAiReportCommand
{
    public string ReportName { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public int QueryId { get; set; }
}
