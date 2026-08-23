using Microsoft.AspNetCore.Components.Forms;
using Silo.Application.Dto;
using Silo.Application.Dto.Filter;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;
public partial class FormatSection<TColumn, TFilter> where TColumn : struct 
                                                     where TFilter : struct
{
    public int SelectedFormat = 0;
    public List<GetReportFormatsByPathVm> ReportFormats;
    public GetReportFormatsByPathVm DeleteFormat = new();
    public CreateReportFormatCommand FormatCommand = new();
    
    [Parameter][EditorRequired] public List<ReportColumnGeneric<TColumn>> DataColumns { get; set; }
    [Parameter][EditorRequired] public List<ReportCalculatingColumn<TColumn>> CalculatingColumns { get; set; }
    [Parameter][EditorRequired] public List<ReportFilterGeneric<TFilter>> Filters { get; set; }
    [Parameter][EditorRequired] public ReportColumnGeneric<TColumn> PivotColumn { get; set; }

    [Parameter] public EventCallback<GetReportFormatsByPathVm> OnChooseFormat { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; }
   
    [CascadingParameter] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ReloadFormats();
    }

    public async Task OnChooseFormatClick(GetReportFormatsByPathVm format)
    {
        await OnChooseFormat.InvokeAsync(format);
    }

    public async Task OnChooseFormatFromDropDownClick(object e)
    {
        int id = (int)e;

        await OnChooseFormat.InvokeAsync(ReportFormats.FirstOrDefault(p=>p.Id == id));

        SelectedFormat = 0;
    }

    public async Task OnDeleteFormatClick(GetReportFormatsByPathVm format)
    {
        DeleteFormat = format;
    }

    public async Task OnLinkFormatClick(GetReportFormatsByPathVm format)
    {
         
    }

    public async Task OnDeleteFormatConfirmClick(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>("wms/ReportFormat"
                                                    , "SDeleteReportFormat"
                                                    , new KeyValuePair<string, object>("id", DeleteFormat.Id))).Value;

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
        IsLoading = true;

        foreach (var item in CalculatingColumns)
        {
            FormatCommand.Details.Add(new()
            {
                DetailType = ReportFormatDetailTypes.Calculating,
                Id = item.Id.ToString(),
                AdditionalData = item.AdditionalData
            });
        }

        foreach (var item in DataColumns)
        {
            FormatCommand.Details.Add(new()
            {
                DetailType = ReportFormatDetailTypes.Data,
                Id = item.Id.ToString(),
                AdditionalData = item.AdditionalData,
                SortType = item.SortType
            });
        }

        foreach (var filter in Filters)
        {
            FormatCommand.Details.Add(new()
            {
                DetailType = ReportFormatDetailTypes.Filter,
                Id = filter.FilterId.ToString(),
                Value = filter.Value,
                AdditionalData = filter.AdditionalData
            });
        }

        if (PivotColumn is not null)
        {
            FormatCommand.Details.Add(new()
            {
                DetailType = ReportFormatDetailTypes.Pivot,
                Id = PivotColumn.Id.ToString(),
                AdditionalData = PivotColumn.AdditionalData
            });
        }

        FormatCommand.Path = new Uri(NavigationManager.Uri).LocalPath.Replace("/", "-");

        FormatCommand.Type = ReportFormatTypes.Column;

        bool result = (await Api.PostAsyncByUri<bool>("wms/ReportFormat"
                               , "SCreateReportFormat"
                               , new KeyValuePair<string, object>("command", FormatCommand))).Value;

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
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        foreach (var message in context.GetValidationMessages())
        {
            Notification.Show(message, "error");
        }
    }

    private async Task ReloadFormats()
    {
        IsLoading = true;

        ReportFormats = (await Api.PostAsyncByUriAndContext<List<GetReportFormatsByPathVm>>("wms/ReportFormat"
                                                                                          , "SGetReportFormatByPath"
                                                                                          , new GetReportFormatsByPathVmContext()
                                                                                          , new KeyValuePair<string, object>("path", new Uri(NavigationManager.Uri).LocalPath.Replace("/", "-")))).Value;
        IsLoading = false;
    }
}
