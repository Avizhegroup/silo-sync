# ImageAnalize page: coding-agent guide

This document describes the current implementation of the warehouse image-analysis page. It is intended for an agent that has no prior knowledge of this feature.

## Purpose and route

- **Route:** `/warehouse/imageanalize`
- **UI:** Blazor Server
- **Markup:** `Silo/Pages/Warehouse/ImageAnalize.razor`
- **Code-behind:** `Silo/Pages/Warehouse/ImageAnalize.razor.cs`
- **Base class:** `Silo.Identity.Client/Base/SiloBasePage.cs`
- **3D implementation:** `Silo/wwwroot/js/babylon/warehouse3D.js`
- **Source styles:** `Silo/wwwroot/styles/pages/warehouse/image-analysis.scss`

The page is an interactive 3D floor-plan editor. At the top level it displays all warehouses as draggable boxes. Selecting a warehouse replaces that view with draggable zones belonging to that warehouse. Users can inspect capacity/status information, reposition warehouses and zones, and create, delete, or relocate corridor geometry. Positions and corridors are persisted through the v2 API.

Despite the class name, this page does not currently perform image recognition or image processing. “Image analysis” means an interactive visual representation of warehouse layout data.

## User scenario

### 1. Entering the page

1. The generated page initialization code checks access through `SiloBasePage.CheckAccess()`.
2. The generated initializer subscribes to the navbar filter-toggle event, resolves the page title, and calls `ImageAnalize.SiloInitializer()`.
3. `SiloInitializer()` sets a Persian title and starts `RefreshWarehousesData()`.
4. A Telerik loading overlay remains visible while warehouse, zone, and corridor data is fetched.
5. On first render, `InitializeCanvas3D()` dynamically loads the Babylon bundle and initializes the canvas.
6. Data and Babylon initialization may finish in either order. `OnAfterRenderAsync()` and `RefreshWarehousesData()` coordinate through `_3DInitialized` and `_dataLoadedAfterInit` so `Update3DView()` runs after both are ready.

The source-generated lifecycle is in `Silo.Source/PropertyCheck/PageAccessGenerator.cs`. It supplies `OnInitializedAsync()` to this partial page class.

### 2. Top-level warehouse view

When `SelectedWarehouse` is `null`, `Update3DView()` sends all warehouses to `Warehouse3D.loadWarehouses()` and sends corridors whose `ContextKey` is an empty string to `Warehouse3D.loadCorridors()`.

Each warehouse appears as an 8×5×8 Babylon box with:

- A billboard label showing `DestinationTitle`.
- A material color based on `OperationalType`.
- A hover tooltip showing title, code, operational type, and active status.
- X/Z-plane drag behavior.
- Click behavior that opens the warehouse's zone view.

If a saved coordinate string exists, JavaScript restores it. Otherwise boxes are arranged automatically in a square grid with 12 scene units of spacing.

### 3. Selecting a warehouse and viewing zones

A click on a warehouse mesh calls the `[JSInvokable]` method `OnWarehouseSelected3D(warehouseCode)`. That resolves the matching VM and calls `SelectWarehouse()`.

`SelectWarehouse()`:

- Assigns `SelectedWarehouse`.
- Cancels corridor drawing if it is active.
- Calls `Update3DView()`.

The page filters zones by `StoreCode == SelectedWarehouse.DestinationCode`, then calls `Warehouse3D.loadZones()`. It also loads corridors whose `ContextKey` equals the selected warehouse code.

Zones are arranged in a 5-unit grid unless saved coordinates exist. Their colors represent occupancy:

| Occupancy | Color |
|---|---|
| Capacity or occupied capacity is zero, or up to 25% | Light gray |
| More than 25% and up to 50% | Yellow |
| More than 50% and up to 75% | Orange |
| More than 75% | Red |

Hovering a zone displays title, code, total capacity, occupied capacity, and percentage.

There is currently no visible “back to warehouses” control. `Warehouse3D.backToWarehouses()` exists and is exported, but the C# page does not call it. Reloading or revisiting the route returns to the top-level view.

### 4. Moving warehouses and zones

Babylon's `PointerDragBehavior` constrains boxes to the X/Z plane. At drag end, JavaScript invokes:

`OnCubePositionChanged(code, type, x, y, z)`

The C# callback creates a comma-separated `x,y,z` coordinate string and immediately calls one of these APIs:

- Warehouse: `PUT Destination/SaveCoordinates`
- Zone: `PUT Zone/SaveCoordinates`

The local VM is then updated. On a later page load, that coordinate string is returned with the warehouse/zone data and parsed by `warehouse3D.js` to restore the mesh position.

`RefreshCubePositionsAsync()` can read all currently visible positions from `Warehouse3D.getPositions()`, but it is not called by the current markup and does not persist them.

### 5. Camera and fullscreen controls

The control stack at the lower-left of the canvas provides:

- Fullscreen: calls `Warehouse3D.toggleFullscreen("warehouse3DContainer")` and uses the browser Fullscreen API.
- Zoom in: subtracts 5 from the ArcRotateCamera radius.
- Zoom out: adds 5 to the radius.
- Corridor mode: enables/disables two-click corridor drawing.

The camera also supports Babylon's attached pointer and wheel controls. Its radius is clamped from 10 to 100.

### 6. Drawing corridors

Corridors are scoped to the view in which they are created:

- Empty `ContextKey`: visible in the top-level warehouse view.
- Warehouse code as `ContextKey`: visible only in that warehouse's zone view.

Workflow:

1. The user enables corridor mode.
2. A range input selects width from 0.5 m to 10 m in 0.5 m steps; the C# default is 1.5 m.
3. JavaScript disables left-button camera rotation while drawing.
4. The first ground click creates a yellow start marker.
5. Pointer movement draws a translucent live preview tube.
6. The second click invokes `OnCorridorDrawn(x1, z1, x2, z2, width)`.
7. C# posts `SaveWarehouseCorridorCommand` to `WarehouseCorridor/Save`.
8. On success, the returned identity ID and geometry are added to `Corridors`, and all corridors for the current context are re-rendered.

Saved corridors are green Babylon tube meshes. Width is treated as tube diameter because JavaScript uses `width / 2` as the tube radius. Draw mode remains enabled after a corridor is saved, allowing consecutive corridors to be added.

### 7. Corridor context menu, deletion, and relocation

Right-clicking a corridor invokes `OnCorridorRightClicked(id, clientX, clientY)`, which opens a Telerik context menu at the pointer position. The menu has Persian options for delete and relocate.

**Delete**

1. `OnCorridorDeleteRequested()` asks for confirmation with `DialogFactory`.
2. It calls `DELETE WarehouseCorridor/Delete/{id}`.
3. On success, it removes the item from the C# list and calls `Warehouse3D.removeCorridorMesh(id)`.

**Relocate**

1. `StartCorridorRelocate()` displays a fixed banner and calls `Warehouse3D.startCorridorRelocate(id)`.
2. Box dragging and left-button camera rotation are disabled.
3. The user selects two new ground points with a live preview.
4. JavaScript invokes `OnCorridorRelocated(id, x1, z1, x2, z2)`.
5. C# deletes the original corridor, removes its mesh, then saves a new corridor with the old context, width, and label but new endpoints.
6. The replacement gets a new database ID and the current context is re-rendered.

The banner's cancel button calls `CancelCorridorRelocate()` and restores camera/box interaction.

## Razor UI and UX structure

`ImageAnalize.razor` contains:

- `PageTitle` from the base page.
- A conditional header controlled by inherited `IsFiltersShown`.
- `#warehouse3DContainer`, containing `#warehouse3DCanvas` and a shared tooltip element.
- Fullscreen, zoom, and corridor controls using `MaterialIcon`.
- A width slider shown only during corridor drawing.
- A relocation banner shown only during relocation.
- `TelerikContextMenu<CorridorContextMenuItem>` for corridor actions.
- `TelerikLoaderContainer` bound to `IsLoading`.

The application is RTL/Persian (`Silo/Pages/_Layout.cshtml` uses `lang="fa" dir="rtl"`). Most page and JavaScript messages are hardcoded Persian; the loader alone uses `TextResources`.

The navbar's generated event hookup toggles `IsFiltersShown`. On this page that flag only hides/shows the header; there is no filter form.

### Styling

The authored SCSS is `Silo/wwwroot/styles/pages/warehouse/image-analysis.scss`, imported through the page/style SCSS chain and compiled into `Silo/wwwroot/styles/Site.css` and `Site.min.css`.

The active 3D styles provide:

- A responsive canvas container with dark gradient, cyan border, and a minimum height.
- A top-right overlay tooltip.
- A bottom-left vertical control stack.
- Fullscreen sizing rules.
- Reduced heights and tooltip sizing below 900 px and 600 px.

Do not hand-edit `Site.css` or `Site.min.css` when changing source styles; change the SCSS and rebuild the generated CSS assets according to the repository's frontend workflow.

Current markup classes `btn-zoom-active`, `corridor-width-control`, and `corridor-relocate-banner` do not have matching rules in the authored page SCSS. Bootstrap styles only partially cover the banner's button.

## JavaScript and library architecture

### Bundle

`Silo/bundleconfig.json` creates the non-minified `Silo/wwwroot/js/Babylon-bundle.js` from:

1. `babylon.js`
2. `babylonjs.loaders.min.js`
3. `babylonjs.materials.min.js`
4. `babylon.gui.min.js`
5. `babylon/warehouse3D.js`

The bundle is intentionally loaded only for this page. `InitializeCanvas3D()` calls the global `loadScript()` helper from `Silo/wwwroot/js/site.js`; that helper appends a script tag with ID `babylon-bundle-script` and resolves when `Warehouse3D` is available.

### `Warehouse3D` module

`warehouse3D.js` is an IIFE that exposes a global `Warehouse3D` object. Its public API is:

| JavaScript method | Called by C# for |
|---|---|
| `initialize` | Create engine, scene, camera, lighting, ground, pointer handlers, and render loop |
| `loadWarehouses` | Render top-level warehouse boxes |
| `loadZones` | Render zones for one warehouse |
| `loadCorridors` | Replace corridor meshes for the current context |
| `setCorridorDrawMode` | Enable/disable two-point drawing |
| `setCorridorWidth` | Update width used by preview/new corridors |
| `removeCorridorMesh` | Remove one deleted corridor |
| `startCorridorRelocate` | Begin selecting replacement endpoints |
| `cancelCorridorRelocate` | Cancel relocation and restore controls |
| `getPositions` | Return visible warehouse/zone coordinates |
| `zoom` | Change camera radius |
| `toggleFullscreen` | Enter/exit browser fullscreen |
| `dispose` | Dispose the engine and clear module state |
| `backToWarehouses` | Return a reload signal; currently unused by C# |

JavaScript calls C# through the `DotNetObjectReference<ImageAnalize>` passed to `initialize()`:

| C# callback | JavaScript trigger |
|---|---|
| `OnWarehouseSelected3D` | Click a warehouse mesh |
| `OnCubePositionChanged` | Finish dragging a warehouse or zone |
| `OnCorridorDrawn` | Select the second endpoint of a new corridor |
| `OnCorridorRightClicked` | Right-click a corridor mesh |
| `OnCorridorRelocated` | Select the second replacement endpoint |

`OnCorridorDeleteRequested` is also `[JSInvokable]`, but the current JavaScript does not invoke it directly; deletion is initiated by the Telerik menu.

### Scene details

- Engine: `BABYLON.Engine` with antialiasing, stencil, and preserved drawing buffer.
- Camera: `BABYLON.ArcRotateCamera` centered at the origin.
- Lights: one hemispheric and one directional light.
- Ground: 100×100 mesh with `BABYLON.GridMaterial`.
- Labels: `BABYLON.GUI.AdvancedDynamicTexture` on billboard planes.
- Interaction: scene pointer observable, mesh action managers, and `PointerDragBehavior`.
- Corridor picking: browser `contextmenu` event plus `scene.pick()`.

## Data loading and API flow

`RefreshWarehousesData()` loads three datasets sequentially:

| Data | Page call | HTTP behavior | Server source |
|---|---|---|---|
| Warehouses | `FormalCache.GetWarehouses()` | On cache miss, `POST RfidCore/v2/Wms/PostObject` with method `SGetAllWarehouses` | `WmsBusiness.SGetAllWarehouses()` maps `WmsApiContext.Warehouses` |
| Zones | `Api.PostAsync<List<GetAllZonesVm>>("SGetAllZones")` | `POST RfidCore/v2/Wms/PostObject` with method `SGetAllZones` | `WmsBusiness.SGetAllZones()` maps `WmsApiContext.Zones` |
| Corridors | `POST WarehouseCorridor/GetAll` | `POST RfidCore/v2/WarehouseCorridor/GetAll` | MediatR `GetAllWarehouseCorridorsHandler` queries `WarehouseCorridors` |

`FormalDataCache` stores the warehouse list in `IMemoryCache` for up to one day. Dragging a warehouse updates the page's cached VM instance but does not explicitly replace or invalidate the formal cache. The persistence API updates the database independently.

`RfidConnectApi` builds its base URL from the `RfidConnectApi` configuration section. Without a configured `Uri`, it uses:

`http://{RfidConnectApi:Ip}/RfidCore/v2/`

Legacy method-name calls are wrapped as an `ApiRequest` with `interface = "RestAPI"`, a method name, and parameters, then sent to `Wms/PostObject`. Direct CQRS calls append the supplied controller URI to the same v2 base URL.

## API endpoints and CQRS handlers

All controller routes inherit `RfidCore/v{version:apiversion}/[controller]`.

| Endpoint | Command/query | Handler | Database action |
|---|---|---|---|
| `PUT Destination/SaveCoordinates` | `SaveWarehouseCoordinatesCommand` | `SaveWarehouseCoordinatesHandler` | `ExecuteUpdate` on warehouse by code |
| `PUT Zone/SaveCoordinates` | `SaveZoneCoordinatesCommand` | `SaveZoneCoordinatesHandler` | `ExecuteUpdate` on zone by code |
| `POST WarehouseCorridor/GetAll` | `GetAllWarehouseCorridorsQuery` | `GetAllWarehouseCorridorsHandler` | Project all corridor rows |
| `POST WarehouseCorridor/Save` | `SaveWarehouseCorridorCommand` | `SaveWarehouseCorridorHandler` | Insert and return identity ID |
| `DELETE WarehouseCorridor/Delete/{id}` | `DeleteWarehouseCorridorCommand` | `DeleteWarehouseCorridorHandler` | `ExecuteDeleteAsync` by ID |

Controllers:

- `Silo.Api/Controllers/v2/DestinationController.cs`
- `Silo.Api/Controllers/v2/ZoneController.cs`
- `Silo.Api/Controllers/v2/WarehouseCorridorController.cs`

Handlers are under `Silo.Application.Api/Features/` while commands, queries, and VMs are under `Silo.Application/Features/`.

## Database usage

### Warehouses

- Entity: `Silo.Domains/Entities/Warehouse.cs`
- Table: `dbo.tbl_Destination`
- Key used by this feature: `DestinationCode`
- Position column: `DestinationCoordinates NVARCHAR(512)`
- Stored format: one comma-separated `x,y,z` string

Warehouse reads also use title, active status, and operational type. `WmsProfile` maps the entity to `WarehouseDto`; the UI deserializes the compatible response into `GetAllWarehousesVm`.

### Zones

- Entity: `Silo.Domains/Entities/Zone.cs`
- Table: `dbo.tbl_Zones`
- Lookup used for position updates: `ZoneCode`
- Warehouse relationship/filter: `ZoneStoreCode`
- Position column: `ZoneCoordinates NVARCHAR(512)`
- Capacity fields: `ZoneCapacity` and `ZoneOccupiedCapacity`

### Warehouse corridors

- Entity: `Silo.Domains/Entities/WarehouseCorridor.cs`
- Table: `dbo.tbl_WarehouseCorridor`
- DbSet: `WmsApiContext.WarehouseCorridors`
- Primary key: identity `fld_WarehouseCorridorId`
- Geometry: `X1`, `Z1`, `X2`, `Z2`, and `Width`, stored as SQL `REAL`
- Scope: `fld_WarehouseCorridorContextKey`
- Optional text: `fld_WarehouseCorridorLabel`

The table definition is `Silo.Api.Db/Tables/tbl_WarehouseCorridor.sql`.

## Important C# methods

| Method | Responsibility |
|---|---|
| `SiloInitializer` | Set title and load all datasets |
| `OnAfterRenderAsync` | Initialize Babylon and render once data is ready |
| `InitializeCanvas3D` | Load bundle, create .NET reference, initialize scene |
| `RefreshWarehousesData` | Load warehouses, zones, and corridors |
| `Update3DView` | Choose warehouse or zone projection and context corridors |
| `SelectWarehouse` | Enter zone view and cancel drawing if necessary |
| `OnCubePositionChanged` | Persist dragged warehouse/zone coordinates |
| `ToggleFullscreen`, `ZoomIn`, `ZoomOut` | Camera/container controls |
| `ToggleCorridorDrawMode`, `OnCorridorWidthChanged` | Corridor editor state |
| `OnCorridorDrawn` | Persist a new corridor |
| `OnCorridorRightClicked`, `OnCorridorMenuItemClick` | Open/dispatch Telerik menu actions |
| `StartCorridorRelocate`, `CancelCorridorRelocate`, `OnCorridorRelocated` | Relocation workflow |
| `OnCorridorDeleteRequested` | Confirm and delete a corridor |
| `DisposeAsync` | Dispose Babylon/.NET references and remove the script tag |

## Current limitations and implementation hazards

These are current behaviors to understand before modifying the page:

1. **No back navigation inside the 3D view.** Selecting a warehouse is one-way until reload/revisit.
2. **Relocation is delete-then-insert, not an update.** The ID changes. If deletion succeeds and insertion fails, the corridor is lost; there is no transaction across the two HTTP calls.
3. **Coordinate serialization is culture-sensitive.** C# uses `$"{x},{y},{z}"` and JavaScript splits on commas. A culture that formats decimals with commas can produce an ambiguous string. Use an invariant, structured representation if this is revised.
4. **Position-save results are ignored.** `OnCubePositionChanged()` updates the local VM even when the API response indicates failure, so the screen can temporarily disagree with the database.
5. **Warehouse cache is not invalidated after a move.** The in-memory VM is changed, but no explicit `UpdateWarehouses()` or hard refresh is performed.
6. **Empty zone handling can leave the previous scene visible.** C# only enters its zone branch when the global `Zones` list has items, and `loadZones()` returns before clearing when its filtered input is empty.
7. **Reload suppression uses collection count.** `loadWarehouses()`/`loadZones()` can skip rendering when item count and view identity are unchanged even if item data changed.
8. **Hardcoded UI text.** Most labels, tooltips, confirmations, and menu text bypass `TextResources`.
9. **Tooltip content uses `innerHTML`.** Warehouse/zone titles and corridor labels are interpolated without HTML encoding.
10. **Corridor hover text is misleading.** It says “click to delete,” but actual actions require right-click and a context-menu selection.
11. **Corridor controls lack authored styles.** The active button, width panel, and relocation banner classes are present in markup but absent from the page SCSS.
12. **Unused/currently disconnected code exists.** `_module`, `Warehouse`, `RelocatingCorridorId`, the local `single` value in `OnCorridorDrawn()`, `RefreshCubePositionsAsync()`, and JavaScript `backToWarehouses()` are not required by the current visible flow.
13. **Script cleanup is partial.** Disposal removes the script element, but the global `Warehouse3D` symbol remains. The anonymous window resize listener created by `initialize()` is not explicitly removed.
14. **Errors are mostly silent to users.** Exceptions are logged as warnings, but the page does not show an error notification or retry UI.
15. **No automated tests were found for this page flow.** Changes should be verified manually in both warehouse and zone contexts.

## Safe change checklist

When changing this feature:

1. Preserve the race-safe contract between data loading and first-render Babylon initialization.
2. Keep C# anonymous-object property names compatible with JavaScript (`destinationCode`, `zoneCode`, `x1`, `z1`, and so on).
3. Keep `[JSInvokable]` names and argument order synchronized with `invokeMethodAsync()` calls.
4. Rebuild `Babylon-bundle.js` after editing `warehouse3D.js`; the browser loads the bundle, not the source file directly.
5. Rebuild generated CSS after editing `image-analysis.scss`.
6. Test top-level and selected-warehouse corridor scoping separately.
7. Test drag persistence by reloading the page after moving both a warehouse and a zone.
8. Test corridor create, cancel, delete, and relocate, including API-failure behavior.
9. Test fullscreen and responsive layouts.
10. Dispose/navigate away and revisit the route to catch JavaScript listener or engine lifecycle issues.

## Manual smoke-test sequence

1. Open `/warehouse/imageanalize` with an authorized user.
2. Confirm the loader disappears and warehouse boxes/corridors render.
3. Hover warehouses and verify tooltip values.
4. Drag one warehouse, reload, and verify its saved position.
5. Select a warehouse and verify only its zones and scoped corridors appear.
6. Hover zones and verify occupancy colors and tooltip calculations.
7. Drag one zone, reload/re-enter the warehouse, and verify its saved position.
8. Test zoom and fullscreen enter/exit.
9. Draw corridors at minimum, default, and maximum widths.
10. Right-click a corridor, cancel deletion, then confirm deletion.
11. Relocate a corridor, verify its ID replacement and position after reload.
12. Cancel relocation and verify box/camera controls are restored.
13. Navigate away and return to ensure the Babylon scene initializes again.
