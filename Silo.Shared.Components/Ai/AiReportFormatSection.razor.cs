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
    [Parameter][EditorRequired] public int QueryId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ReloadFormats();
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (string.IsNullOrWhiteSpace(FormatCommand.ReportName))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");
            return;
        }

        if (QueryId <= 0)
        {
            Notification.Show("شناسه کوئری معتبر نیست", "error");
            return;
        }

        IsSaving = true;

         FormatCommand.Mode = Mode;
            FormatCommand.QueryId = QueryId;

            var saveResult = (await Api.PostAsyncByUri<int>(
                "wms/ReportFormat",
                "SSaveAiReport",
                new KeyValuePair<string, object>("command", FormatCommand))).Value;

        if (saveResult > 0)
        {
            var linkResult = (await Api.PostAsyncByUri<bool>(
                "wms/ReportFormat",
                "SSaveAiReportLink",
                new KeyValuePair<string, object>("reportFormatId", saveResult),
                new KeyValuePair<string, object>("reportName", FormatCommand.ReportName),
                new KeyValuePair<string, object>("userIds", new List<string>()))).Value;

            FormatCommand = new();

            await ReloadFormats();

            IsSaving = false;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            await OnSaved.InvokeAsync();
        }
        else
        {
            IsSaving = false;

            Notification.Show(
                TextResources.APP_StringKeys_Alert_Fail,
                "error");
        }
    }
      
    

    public Task OnInvalidSubmit(EditContext context)
    {
        foreach (var message in context.GetValidationMessages())
            Notification.Show(message, "error");

        return Task.CompletedTask;
    }

    public void OnDeleteFormatClick(GetReportFormatsByPathVm format)
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

    private async Task ReloadFormats()
    {
            if (Api is null)
            {
                ReportFormats = new();
                return;
            }

            var response = await Api.PostAsyncByUri<List<GetReportFormatsByPathVm>>(
                "wms/ReportFormat",
                "SGetAiReportFormats");

            ReportFormats = response?.Value ?? new();
    }
}

