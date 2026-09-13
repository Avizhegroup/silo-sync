using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Silo.Application.Dto;
using Silo.Identity.Client;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Ai;

public partial class SaveAiReportModal
{
    public bool IsLoading = false;
    public string ReportName = string.Empty;
    //public int? SelectedCategoryId = null;
    public List<UserChoosableDto> AllUsers { get; set; } = new();

    public IEnumerable<UserChoosableDto> SelectedUsers { get; set; } = new List<UserChoosableDto>();
    public List<string> SelectedUserIds { get; set; } = new();

    //public List<NavbarAllTitle> MenuCategories = new();
    public Modal Modal { get; set; }

    [Parameter][EditorRequired] public List<object> ReportData { get; set; }

    [Parameter][EditorRequired] public string Mode { get; set; }
    [Parameter] public int? QueryReferenceId { get; set; }
    [Parameter] public EventCallback<bool> OnSaveSuccess { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IClaimManager ClaimManager { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUsers();
       // await LoadMenuCategories();
    }

    public async Task Open(MouseEventArgs e)
    {
        ReportName = string.Empty;

        SelectedUsers = new List<UserChoosableDto>(AllUsers);

        await Modal.Open(e);
    }


    public async Task OnValidSubmit()
    {
        //if (SelectedCategoryId is null)
        //{
        //    Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");
        //    return;
        //}

        if (!SelectedUsers.Any(u => u.IsChoosed))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");
            return;
        }

        IsLoading = true;

        try
        {
            // var reportDataJson = System.Text.Json.JsonSerializer.Serialize(ReportData);
            Notification.Show($"SQL: {QueryReferenceId}", "info");

            var saveResult = (await Api.PostAsyncByUri<int>("wms/ReportFormat", "SSaveAiReport"
           , new KeyValuePair<string, object>("command", new
           {
             ReportName = ReportName,
             Mode = Mode,
             QueryId = QueryReferenceId ?? 0
           })
           )).Value;

            if (saveResult > 0)
            {
                var userIds = SelectedUsers.Where(u => u.IsChoosed).Select(u => u.Id).ToList();

                var linkResult = (await Api.PostAsyncByUri<bool>("wms/ReportFormat"
                    , "SSaveAiReportLink"
                    , new KeyValuePair<string, object>("reportFormatId", saveResult)
                    , new KeyValuePair<string, object>("reportName", ReportName)
                    //, new KeyValuePair<string, object>("categoryId", SelectedCategoryId)
                    , new KeyValuePair<string, object>("userIds", userIds))).Value;

                if (linkResult)
                {
                    Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
                    await Modal.Close(null);
                    await OnSaveSuccess.InvokeAsync(true);
                }
                else
                {
                    Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
                }
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
            IsLoading = false;
        }
    }

    public Task OnInvalidSubmit()
    {
        Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");
        return Task.CompletedTask;
    }

    private async Task LoadUsers()
    {
        var applicationUsers =
            (await Api.PostAsync<List<ApplicationUser>>("GetAllUser")).Value;

        AllUsers = applicationUsers?
            .Where(p => p.IsActive)
            .Select(p => new UserChoosableDto
            {
                Id = p.Id,
                Name = p.Name,
                UserName = p.UserName,
                IsChoosed = true
            })
            .ToList()
            ?? new List<UserChoosableDto>();

        SelectedUsers = new List<UserChoosableDto>(AllUsers);
    }

    //private async Task LoadMenuCategories()
    //{
    //    var allLinks = await ClaimManager.GetAllLinks();
    //    MenuCategories = allLinks;
    //}

}

public class UserChoosableDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public bool IsChoosed { get; set; }
}

