using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Silo.Application.Features;
using Silo.Components.LiftTruck;

namespace Silo.Pages.LiftTruck;

public partial class Index
{
    public bool IsLoading = true;
    public TruckCargoDto Cargo;
    public string CurrentDateTime = $"{PersianCalendarTools.PersianDayName(DateTime.Now)} {PersianCalendarTools.GregorianToPersian(DateTime.Now)}";
    public string? TruckNumber;
    public string? UserId;
    public string MainDivClass = string.Empty;
    public TruckIndexMode PageActiveMode = TruckIndexMode.Default;
    public HubConnection HubConnection;
    public HubConnectionState HubConnectionState = HubConnectionState.Disconnected;

    [Inject] public ProtectedLocalStorage Storage { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject] public ILogger<Index> Logger { get; set; }

    public TruckSettings TruckSetting { get; set; }
    public TruckProductCode TruckProductCode { get; set; }
    public TruckConfirm Confirm { get; set; }
    public TruckError TruckError { get; set; }
    public TelerikLoaderContainer LoadingContainer { get; set; }
    public TruckDefault TruckDefault { get; set; }

    protected override async Task SiloInitializer()
    {
        await InitSignalR();

        var truckStorageResult = await Storage.GetAsync<string>("truck");

        UserId = (await SiloAuth.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = false;

        if (!truckStorageResult.Success)
        {
            await OnChangePageActiveModeClick(TruckIndexMode.Settings);

            return;
        }

        TruckNumber = truckStorageResult.Value;
    }

    public async Task OnChangePageActiveModeClick(TruckIndexMode type)
    {
        RestateToDefault();

        switch (type)
        {
            case TruckIndexMode.Settings:
                TruckSetting.Show();

                TruckDefault.Hide();

                PageActiveMode = TruckIndexMode.Settings;

                break;
            case TruckIndexMode.List:
                TruckProductCode.Show();

                TruckDefault.Hide();

                PageActiveMode = TruckIndexMode.List;

                break;
            case TruckIndexMode.Default:
                PageActiveMode = TruckIndexMode.Default;

                break;
        }
    }

    public async Task OnLogoClick(MouseEventArgs e)
    {
        NavigationManager.NavigateTo("/truck/index", true);
    }

    public async Task OnSettingsSave(TruckConfigDto config)
    {
        TruckNumber = config.TruckNumber;

        RestateToDefault();
    }

    public async Task OnLogoutClick(MouseEventArgs e)
    {
        await SiloAuth.SetUserLoggedOut();
    }

    public async Task OnGetProductDetails(string productCode)
    {
        if (Cargo is not null)
        {
            await Api.PostAsync<string>("SGetPlacementMission"
            , new("ProductSerials", new string[]
            {
                productCode
            })
            , new("Epcs", new string[0])
            , new("WMDriverUserId", UserId)
            , new("WMId", TruckNumber)
            , new("ActionId", Cargo.GateActionId.HasValue() ? Cargo.GateActionId : "-1")
            , new("GateNumber", "-1")
            , new("GateTitle", "-1")
            , new("TypeGetPlacementMission", "1")
            , new("ActionDescription", "-1")
            , new("ActionStatus", "-1")
            , new("RecursiveFunction", false)
            , new("castResult", true));
        }
        else
        {
            await Api.PostAsync<string>("SGetPlacementMission"
            , new("ProductSerials", new string[]
            {
                productCode
            })
            , new("Epcs", new string[0])
            , new("WMDriverUserId", UserId)
            , new("WMId", TruckNumber)
            , new("ActionId", "-1")
            , new("GateNumber", "-1")
            , new("GateTitle", "-1")
            , new("TypeGetPlacementMission", "1")
            , new("ActionDescription", "-1")
            , new("ActionStatus", "-1")
            , new("RecursiveFunction", false)
            , new("castResult", true));
        }

        await OnChangePageActiveModeClick(TruckIndexMode.Default);
    }

    public async Task OnReconnectClick(MouseEventArgs e)
    {
        await HubConnection.StartAsync();

        HubConnectionState = HubConnectionState.Connected;
    }

    public async Task OnSignalrError(Exception? exception, HubConnectionState state)
    {
        Logger.LogError(exception, exception.Message);

        HubConnectionState = state;

        StateHasChanged();
    }

    public async Task OnReceiveProductOfGate(string productString)
    {
        TruckCargoDto receievedCargo = (JsonConvert.DeserializeObject<TruckCargoDto>(productString));

        if (receievedCargo.DriverUserId.Equals(UserId))
        {
            if (receievedCargo.ActionStatus != ActionStatus.Error)
            {
                if (Cargo is null)
                {
                    Cargo = receievedCargo;

                    TruckDefault.Show(Cargo);
                }
                else
                {
                    if (receievedCargo.GateActionId.Equals(Cargo.GateActionId))
                    {
                        Cargo = receievedCargo;

                        TruckDefault.Show(Cargo);
                    }
                    else
                    {
                        Notification.Show(TextResources.APP_StringKeys_Validation_Cargo_Status, "error");
                    }

                }
            }
            else
            {
                MainDivClass = "danger";

                PageActiveMode = TruckIndexMode.Error;

                TruckDefault.Hide();

                TruckError.Show(receievedCargo.ActionDescription);

                StateHasChanged();
            }
        }
    }

    public async Task OnVerifyClick()
    {
        TruckDefault.Hide();

        MainDivClass = "warning";

        await Confirm.Show(Cargo);
    }

    public async Task OnConfirmChange(TruckConfirmMode mode)
    {
        switch (mode)
        {
            case TruckConfirmMode.Verify or TruckConfirmMode.Cancel:
                bool result = (await Api.PostAsync<bool>("SPlacementMissionResult"
                  , new("PMCode", Cargo.Products.Select(p => p.PmCode).ToArray())
                  , new("Status", mode == TruckConfirmMode.Verify ? "2" : "3")
                  , new("ToStoreCode", Cargo.DestinationWarehouseCode)
                  , new("ToZoneId", Cargo.DestinationZoneCode))).Value;

                if (result)
                {
                    Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

                    Cargo = null;
                }
                else
                {
                    Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
                }
                break;
            case TruckConfirmMode.Back:
                break;
        }

        RestateToDefault();
    }

    public async Task OnPageModeChange(TruckIndexMode mode)
    {
        RestateToDefault();

        switch (mode)
        {
            case TruckIndexMode.Settings:
                TruckSetting.Show();

                TruckDefault.Hide();
                break;
            case TruckIndexMode.List:
                break;
            case TruckIndexMode.Keyboard:
                TruckProductCode.Show();

                TruckDefault.Hide();
                break;
            case TruckIndexMode.Rfid:
                break;
        }
    }

    private void RestateToDefault()
    {
        TruckSetting.Hide();

        TruckProductCode.Hide();

        Confirm.Hide();

        if (Cargo is not null)
        {
            TruckDefault.Show(Cargo);
        }
        else
        {
            TruckDefault.Show();
        }

        MainDivClass = string.Empty;

        StateHasChanged();
    }

    private async Task InitSignalR()
    {
        HubConnection = new HubConnectionBuilder()
            .WithUrl($"http://{Configuration.GetSection("RfidConnectApi")["Ip"]}/wmshub")
            .WithAutomaticReconnect()
            .Build();

        await HubConnection.StartAsync();

        HubConnectionState = HubConnectionState.Connected;

        Console.WriteLine("Signal connected!");

        HubConnection.On<string>("GateIdent", OnReceiveProductOfGate);

        HubConnection.Reconnecting += async (exception) => await OnSignalrError(exception, HubConnectionState.Reconnecting);

        HubConnection.Closed += async (exception) => await OnSignalrError(exception, HubConnectionState.Disconnected);
    }
}
