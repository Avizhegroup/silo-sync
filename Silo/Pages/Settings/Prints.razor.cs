using Silo.Application;

namespace Silo.Pages.Settings;
public partial class Prints
{
    public bool IsLoading = false;
    public Dictionary<string, string> Files = new();

    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Exporter { get; set; }

    protected override async Task SiloInitializer()
    {
        InitFiles();
    }

    public async Task OnDownloadFileClick(string file)
    {
        string fileName = Path.Combine(Environment.WebRootPath, "reports", file);

        await Exporter.ExportAndDownload(fileName, file);
    }

    private void InitFiles()
    {
        Files.Add("Action.mrt", TextResources.APP_StringKeys_Settings_Print_Action);
        Files.Add("Aggregate.mrt", TextResources.APP_StringKeys_Settings_Print_Aggregate);
        Files.Add("ApiSync.mrt", TextResources.APP_StringKeys_Settings_Print_ApiSync);
        Files.Add("Collect.mrt", TextResources.APP_StringKeys_Settings_Print_Collect);
        Files.Add("Enter.mrt", TextResources.APP_StringKeys_Settings_Print_Enter);
        Files.Add("EnterAction.mrt", TextResources.APP_StringKeys_Settings_Print_EnterAction);
        Files.Add("EnterActionAgg.mrt", TextResources.APP_StringKeys_Settings_Print_EnterActionAgg);
        Files.Add("ExitAction.mrt", TextResources.APP_StringKeys_Settings_Print_ExitAction);
        Files.Add("Inventory.mrt", TextResources.APP_StringKeys_Settings_Print_Inventory);
        Files.Add("InventoryDetails.mrt", TextResources.APP_StringKeys_Settings_Print_InventoryDetails);
        Files.Add("Register.mrt", TextResources.APP_StringKeys_Settings_Print_Register);
    }
}
