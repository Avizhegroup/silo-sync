using System.Net.Http;
using Silo.Application.Features;
using Silo.Infrastructure.Web;

namespace Silo.Pages.Sync;

public partial class Sources
{
    public bool IsLoading = true;
    public List<GetSyncSourcesVm> SourceList { get; set; } = new();
    public CreateSyncSourceCommand Request { get; set; } = new();
    public Modal SourceModal { get; set; } = null!;
    public string SourceModalTitle { get; set; } = "Add Sync Source";

    private int? _editingId;

    [Inject] public RfidConnectApi Api { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task SiloInitializer()
    {
        await LoadSourcesAsync();
        IsLoading = false;
    }

    private async Task LoadSourcesAsync()
    {
        IsLoading = true;
        var response = await Api.SendAsyncObjectByUri<List<GetSyncSourcesVm>>(HttpMethod.Get, "SyncAdmin/sources");
        SourceList = response?.Value ?? new List<GetSyncSourcesVm>();
        IsLoading = false;
    }

    private void OnAddClick()
    {
        _editingId = null;
        Request = new CreateSyncSourceCommand();
        SourceModalTitle = "Add Sync Source";
        SourceModal.Open(new());
    }

    private void OnEditClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        _editingId = source.Id;
        Request = new CreateSyncSourceCommand
        {
            SourceKey = source.SourceKey,
            DisplayName = source.DisplayName,
            SourceType = source.SourceType,
            Command = source.Command,
            FieldKey = source.FieldKey,
            FieldCheck = source.FieldCheck,
            FieldOrder = source.FieldOrder,
            IntervalSeconds = source.IntervalSeconds ?? 60,
            IsEnabled = source.IsEnabled,
            ConnectionString = null
        };
        SourceModalTitle = "Edit Sync Source";
        SourceModal.Open(new());
    }

    private async Task OnSaveSubmit()
    {
        IsLoading = true;

        if (_editingId.HasValue)
        {
            var update = new UpdateSyncSourceCommand
            {
                Id = _editingId.Value,
                SourceKey = Request.SourceKey,
                DisplayName = Request.DisplayName,
                SourceType = Request.SourceType,
                Command = Request.Command,
                FieldKey = Request.FieldKey,
                FieldCheck = Request.FieldCheck,
                FieldOrder = Request.FieldOrder,
                IntervalSeconds = Request.IntervalSeconds,
                IsEnabled = Request.IsEnabled,
                ConnectionString = Request.ConnectionString
            };
            await Api.SendAsyncObjectByUri<UpdateSyncSourceVm>(HttpMethod.Put, "SyncAdmin/sources/" + _editingId.Value, update);
        }
        else
        {
            await Api.SendAsyncObjectByUri<CreateSyncSourceVm>(HttpMethod.Post, "SyncAdmin/sources", Request);
        }

        SourceModal.Close(new());
        await LoadSourcesAsync();
        IsLoading = false;
    }

    private void OnCancelClick()
    {
        SourceModal.Close(new());
    }

    private async Task OnDeleteClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        IsLoading = true;
        await Api.SendAsyncObjectByUri<DeleteSyncSourceVm>(HttpMethod.Delete, "SyncAdmin/sources/" + source.Id, new DeleteSyncSourceCommand { Id = source.Id });
        await LoadSourcesAsync();
        IsLoading = false;
    }

    private async Task OnTestClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        IsLoading = true;
        var response = await Api.SendAsyncObjectByUri<TestSyncSourceQueryVm>(HttpMethod.Post, $"SyncAdmin/sources/{source.Id}/test-query", new TestSyncSourceQueryCommand { Id = source.Id });
        IsLoading = false;

        var result = response?.Value;
        var message = result?.Success == true
            ? $"Test succeeded. Columns: {string.Join(", ", result.Columns ?? new List<string>())}"
            : $"Test failed: {result?.ErrorMessage ?? response?.Messages?.FirstOrDefault()}";
        await JsRuntime.InvokeVoidAsync("alert", message);
    }

    private async Task OnToggleEnabledClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        IsLoading = true;
        var action = source.IsEnabled ? "disable" : "enable";
        await Api.SendAsyncObjectByUri<EnableDisableSyncSourceVm>(HttpMethod.Post, $"SyncAdmin/sources/{source.Id}/{action}");
        await LoadSourcesAsync();
        IsLoading = false;
    }
}
