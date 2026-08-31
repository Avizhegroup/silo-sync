using Silo.Application;

namespace Silo.Pages.Warehouse;
public partial class ImageAnalize : IAsyncDisposable
{
    private bool IsLoading = true;
    private IJSObjectReference? _module;
    private bool _3DInitialized = false;
    private DotNetObjectReference<ImageAnalize>? _dotNetRef;
    private bool _dataLoadedAfterInit = false;
    private bool IsCorridorDrawMode = false;
    private bool IsCorridorRelocateMode = false;
    private int? RelocatingCorridorId = null;
    private float CorridorWidth = 1.5f;
    private TelerikContextMenu<CorridorContextMenuItem> _corridorContextMenu;
    private List<CorridorContextMenuItem> _corridorMenuItems = new()
    {
        new() { Text = "حذف", Icon = "trash", CommandName = "delete" },
        new() { Text = "جابجایی", Icon = "redo", CommandName = "relocate" }
    };
    private int _contextMenuCorridorId;
    private List<GetAllWarehouseCorridorsVm> Corridors = new();
    public GetAllWarehousesVm Warehouse = new();
    public List<GetAllWarehousesVm> Warehouses = new();
    public List<GetAllZonesVm> Zones;
    public GetAllWarehousesVm? SelectedWarehouse;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public ILogger<ImageAnalize> Logger { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    protected override async Task SiloInitializer()
    {
        PageTitle = "آنالیز تصویری انبار";

        await RefreshWarehousesData();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await InitializeCanvas3D();
        }

        if (!IsLoading && !_dataLoadedAfterInit && _3DInitialized)
        {
            await Update3DView();

            _dataLoadedAfterInit = true;
        }
    }

    private async Task InitializeCanvas3D()
    {
        try
        {
            _dotNetRef = DotNetObjectReference.Create(this);

            // Dynamically inject Babylon bundle only for this page and wait until it is ready
            await JSRuntime.InvokeVoidAsync("loadScript", "babylon-bundle-script", "/js/Babylon-bundle.js");

            await JSRuntime.InvokeVoidAsync("Warehouse3D.initialize", "warehouse3DCanvas", _dotNetRef);

            _3DInitialized = true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);

            _3DInitialized = false;
        }
    }

    private async Task Update3DView()
    {
        if (!_3DInitialized)
        {
            return;
        }

        try
        {
            if (SelectedWarehouse is null && Warehouses?.Count > 0)
            {
                // Show warehouses in 3D
                var warehouseData = Warehouses.Select(w => new
                {
                    destinationCode = w.DestinationCode,
                    destinationTitle = w.DestinationTitle,
                    operationalType = (int)w.OperationalType,
                    isActive = w.IsActive,
                    coordinates = w.Coordinates
                }).ToArray();

                await JSRuntime.InvokeVoidAsync("Warehouse3D.loadWarehouses", (object)warehouseData);

                // Render saved corridors after warehouses are placed
                if (Corridors?.Count > 0)
                {
                    var corridorData = Corridors
                        .Where(c => c.ContextKey == string.Empty)
                        .Select(c => new { id = c.Id, x1 = c.X1, z1 = c.Z1, x2 = c.X2, z2 = c.Z2, width = c.Width, label = c.Label })
                        .ToArray();

                    if (corridorData.Length > 0)
                        await JSRuntime.InvokeVoidAsync("Warehouse3D.loadCorridors", (object)corridorData);
                }
            }
            else if (SelectedWarehouse is not null && Zones?.Count > 0)
            {
                // Show zones in 3D
                var zoneData = Zones.Where(z => z.StoreCode == SelectedWarehouse.DestinationCode)
                    .Select(z => new
                    {
                        zoneCode = z.ZoneCode,
                        title = z.Title,
                        capacity = z.Capacity,
                        occupiedCapacity = z.OccupiedCapacity,
                        storeCode = z.StoreCode,
                        coordinates = z.Coordinates
                    }).ToArray();

                await JSRuntime.InvokeVoidAsync("Warehouse3D.loadZones", (object)zoneData, SelectedWarehouse.DestinationCode);

                // Load corridors for this zone context
                var zoneCorridorData = Corridors
                    .Where(c => c.ContextKey == SelectedWarehouse.DestinationCode)
                    .Select(c => new { id = c.Id, x1 = c.X1, z1 = c.Z1, x2 = c.X2, z2 = c.Z2, width = c.Width, label = c.Label })
                    .ToArray();

                if (zoneCorridorData.Length > 0)
                    await JSRuntime.InvokeVoidAsync("Warehouse3D.loadCorridors", (object)zoneCorridorData);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }
    }

    [JSInvokable]
    public async Task OnWarehouseSelected3D(string warehouseCode)
    {
        try
        {
            var warehouse = Warehouses?.FirstOrDefault(w => w.DestinationCode == warehouseCode);

            if (warehouse != null)
            {
                await SelectWarehouse(warehouse);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }
    }

    /// <summary>
    /// Called by JavaScript whenever a cube is dropped at a new position.
    /// </summary>
    [JSInvokable]
    public async Task OnCubePositionChanged(string code, string type, float x, float y, float z)
    {
        var coords = $"{x},{y},{z}";

        if (type == "warehouse")
        {
            var vm = Warehouses?.FirstOrDefault(w => w.DestinationCode == code);
          
            if (vm is not null)
            {
                var result = await Api.SendAsyncObjectByUri<SaveWarehouseCoordinatesVm>(HttpMethod.Put
                    , "Destination/SaveCoordinates"
                    , new SaveWarehouseCoordinatesCommand()
                      {
                          Code = code,
                          Coordinates = coords
                      });

                vm.Coordinates = coords;
            }
        }
        else if (type == "zone")
        {
            var vm = Zones?.FirstOrDefault(z => z.ZoneCode == code);
           
            if (vm is not null) 
            {
                var result = await Api.SendAsyncObjectByUri<SaveZoneCoordinatesVm>(HttpMethod.Put
                    , "Zone/SaveCoordinates"
                    , new SaveZoneCoordinatesCommand()
                    {
                        Code = code,
                        Coordinates = coords
                    });

                vm.Coordinates = coords; 
            }
        }

        StateHasChanged();
    }

    public async Task RefreshCubePositionsAsync()
    {
        try
        {
            var positions = await JSRuntime.InvokeAsync<List<WarehousePosisionin3DViewDto>>("Warehouse3D.getPositions");

            foreach (var p in positions)
            {
                var coords = $"{p.X},{p.Y},{p.Z}";

                if (p.Type == "warehouse")
                {
                    var vm = Warehouses?.FirstOrDefault(w => w.DestinationCode == p.Code);
                 
                    if (vm is not null)
                    {
                        vm.Coordinates = coords;
                    }
                }
                else if (p.Type == "zone")
                {
                    var vm = Zones?.FirstOrDefault(z => z.ZoneCode == p.Code);
                  
                    if (vm is not null)
                    {
                        vm.Coordinates = coords;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }
    }

    public async Task RefreshWarehousesData()
    {
        IsLoading = true;
      
        StateHasChanged();

        Warehouses = await FormalCache.GetWarehouses();

        Zones = (await Api.PostAsync<List<GetAllZonesVm>>("SGetAllZones")).Value;

        Corridors = (await Api.SendAsyncObjectByUri<List<GetAllWarehouseCorridorsVm>>(HttpMethod.Post, "WarehouseCorridor/GetAll")).Value ?? new();

        IsLoading = false;
        
        StateHasChanged();

        if (_3DInitialized)
        {
            await Update3DView();

            _dataLoadedAfterInit = true;
        }
    }

    private async Task SelectWarehouse(GetAllWarehousesVm warehouse)
    {
        SelectedWarehouse = warehouse;

        if (IsCorridorDrawMode)
        {
            IsCorridorDrawMode = false;
            if (_3DInitialized)
                await JSRuntime.InvokeVoidAsync("Warehouse3D.setCorridorDrawMode", false);
        }

        StateHasChanged();

        await Update3DView();
    }

    private async Task ZoomIn()
    {
        if (_3DInitialized)
        { 
            await JSRuntime.InvokeVoidAsync("Warehouse3D.zoom", -5f); 
        }
    }

    private async Task ZoomOut()
    {
        if (_3DInitialized)
        { 
            await JSRuntime.InvokeVoidAsync("Warehouse3D.zoom", 5f);
        }
    }

    private async Task ToggleFullscreen()
    {
        await JSRuntime.InvokeVoidAsync("Warehouse3D.toggleFullscreen", "warehouse3DContainer");
    }

    private async Task ToggleCorridorDrawMode()
    {
        IsCorridorDrawMode = !IsCorridorDrawMode;

        if (_3DInitialized)
        {
            await JSRuntime.InvokeVoidAsync("Warehouse3D.setCorridorDrawMode", IsCorridorDrawMode, (double)CorridorWidth);
        }

        StateHasChanged();
    }

    private async Task OnCorridorWidthChanged(ChangeEventArgs e)
    {
        if (float.TryParse(e.Value?.ToString(), out var w))
        {
            CorridorWidth = w;

            if (_3DInitialized)
                await JSRuntime.InvokeVoidAsync("Warehouse3D.setCorridorWidth", (double)CorridorWidth);

            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task OnCorridorDrawn(float x1, float z1, float x2, float z2, float width)
    {
        try
        {
            var contextKey = SelectedWarehouse?.DestinationCode ?? string.Empty;

            var response = await Api.SendAsyncObjectByUri<SaveWarehouseCorridorVm>(HttpMethod.Post
                , "WarehouseCorridor/Save"
                , new SaveWarehouseCorridorCommand
                {
                    ContextKey = contextKey,
                    X1 = x1,
                    Z1 = z1,
                    X2 = x2,
                    Z2 = z2,
                    Width = width
                });

            var result = response?.Value;

            if (result?.Result == true)
            {
                var newCorridor = new GetAllWarehouseCorridorsVm
                {
                    Id = result.Id,
                    ContextKey = contextKey,
                    X1 = x1,
                    Z1 = z1,
                    X2 = x2,
                    Z2 = z2,
                    Width = width
                };

                Corridors.Add(newCorridor);

                // Immediately render the new corridor in JS
                var single = new[] { new { id = result.Id, x1, z1, x2, z2, width, label = (string?)null } };
                await JSRuntime.InvokeVoidAsync("Warehouse3D.loadCorridors",
                    (object)Corridors
                        .Where(c => c.ContextKey == contextKey)
                        .Select(c => new { id = c.Id, x1 = c.X1, z1 = c.Z1, x2 = c.X2, z2 = c.Z2, width = c.Width, label = c.Label })
                        .ToArray());
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnCorridorRightClicked(int corridorId, double clientX, double clientY)
    {
        _contextMenuCorridorId = corridorId;
        await _corridorContextMenu.ShowAsync(clientX, clientY);
    }

    private async Task OnCorridorMenuItemClick(CorridorContextMenuItem item)
    {
        if (item.CommandName == "delete")
        {
            await OnCorridorDeleteRequested(_contextMenuCorridorId);
        }
        else if (item.CommandName == "relocate")
        {
            await StartCorridorRelocate(_contextMenuCorridorId);
        }
    }

    private async Task StartCorridorRelocate(int corridorId)
    {
        IsCorridorRelocateMode = true;
        RelocatingCorridorId = corridorId;
        StateHasChanged();

        if (_3DInitialized)
            await JSRuntime.InvokeVoidAsync("Warehouse3D.startCorridorRelocate", corridorId);
    }

    public async Task CancelCorridorRelocate()
    {
        IsCorridorRelocateMode = false;
        RelocatingCorridorId = null;
        StateHasChanged();

        if (_3DInitialized)
            await JSRuntime.InvokeVoidAsync("Warehouse3D.cancelCorridorRelocate");
    }

    [JSInvokable]
    public async Task OnCorridorRelocated(int corridorId, float x1, float z1, float x2, float z2)
    {
        IsCorridorRelocateMode = false;
        RelocatingCorridorId = null;

        try
        {
            var existing = Corridors.FirstOrDefault(c => c.Id == corridorId);
            if (existing is null) return;

            // Delete old corridor
            var deleteResponse = await Api.SendAsyncObjectByUri<DeleteWarehouseCorridorVm>(HttpMethod.Delete
                , $"WarehouseCorridor/Delete/{corridorId}"
                , null);

            if (deleteResponse?.Value?.Result != true) return;

            Corridors.RemoveAll(c => c.Id == corridorId);
            await JSRuntime.InvokeVoidAsync("Warehouse3D.removeCorridorMesh", corridorId);

            // Save as new corridor at the updated position
            var saveResponse = await Api.SendAsyncObjectByUri<SaveWarehouseCorridorVm>(HttpMethod.Post
                , "WarehouseCorridor/Save"
                , new SaveWarehouseCorridorCommand
                {
                    ContextKey = existing.ContextKey,
                    X1 = x1,
                    Z1 = z1,
                    X2 = x2,
                    Z2 = z2,
                    Width = existing.Width,
                    Label = existing.Label
                });

            var result = saveResponse?.Value;
            if (result?.Result == true)
            {
                var relocated = new GetAllWarehouseCorridorsVm
                {
                    Id = result.Id,
                    ContextKey = existing.ContextKey,
                    X1 = x1,
                    Z1 = z1,
                    X2 = x2,
                    Z2 = z2,
                    Width = existing.Width,
                    Label = existing.Label
                };

                Corridors.Add(relocated);

                var contextKey = existing.ContextKey;
                await JSRuntime.InvokeVoidAsync("Warehouse3D.loadCorridors",
                    (object)Corridors
                        .Where(c => c.ContextKey == contextKey)
                        .Select(c => new { id = c.Id, x1 = c.X1, z1 = c.Z1, x2 = c.X2, z2 = c.Z2, width = c.Width, label = c.Label })
                        .ToArray());
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }

        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnCorridorDeleteRequested(int corridorId)
    {
        var confirmed = await Dialog.ConfirmAsync("\u0622\u06cc\u0627 \u0645\u06cc\u062e\u0648\u0627\u0647\u06cc\u062f \u0627\u06cc\u0646 \u0631\u0627\u0647\u0631\u0648 \u062d\u0630\u0641 \u0634\u0648\u062f\u061f", "\u062d\u0630\u0641 \u0631\u0627\u0647\u0631\u0648");

        if (!confirmed) return;

        try
        {
            var response = await Api.SendAsyncObjectByUri<DeleteWarehouseCorridorVm>(HttpMethod.Delete
                , $"WarehouseCorridor/Delete/{corridorId}"
                , null);

            if (response?.Value?.Result == true)
            {
                Corridors.RemoveAll(c => c.Id == corridorId);

                await JSRuntime.InvokeVoidAsync("Warehouse3D.removeCorridorMesh", corridorId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }

        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("Warehouse3D.dispose");

            _dotNetRef?.Dispose();

            if (_module is not null)
            {
                await _module.DisposeAsync();
            }

            // Remove the Babylon bundle script tag so it doesn't linger for other pages
            await JSRuntime.InvokeVoidAsync("removeScript", "babylon-bundle-script");
        }
        catch
        {
            // Ignore disposal errors
        }
    }

    }
