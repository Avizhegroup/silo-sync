using Microsoft.Extensions.Options;
using Silo.Service.Sharif.Configuration;
using Silo.Service.Sharif.Services;

namespace Silo.Service.Sharif.Components.Pages;

public partial class Settings
{
    [Inject]
    private IOptionsMonitor<RfidWorkerOptions> Options { get; set; } = null!;

    [Inject]
    private WorkerSettingsWriter SettingsWriter { get; set; } = null!;

    [Inject]
    private ReaderControlService ReaderControl { get; set; } = null!;

    private RfidWorkerOptions Model { get; set; } = new();

    private TelerikNotification? Notification { get; set; }

    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        ReloadModel();
    }

    private async Task OnSaveSubmit()
    {
        IsLoading = true;
        var previousPower = Options.CurrentValue.ReaderPower;

        var saved = await SettingsWriter.SaveAsync(Model);

        if (saved && Model.ReaderPower != previousPower)
        {
            await ReaderControl.SetPowerAsync(Model.ReaderPower);
        }

        IsLoading = false;
        ShowResult(saved);
    }

    private void OnClearClick()
    {
        ReloadModel();
    }

    private void ReloadModel()
    {
        Model = Options.CurrentValue.Clone();
    }

    private void ShowResult(bool succeeded)
    {
        Notification?.Show(new NotificationModel
        {
            ThemeColor = succeeded ? ThemeConstants.Notification.ThemeColor.Success : ThemeConstants.Notification.ThemeColor.Error,
            Text = succeeded ? TextResources.SharifUi_SettingsSaved : TextResources.SharifUi_SettingsSaveFailed,
            CloseAfter = 3000
        });
    }
}
