# RfidConnectRegister (Silo Windows Register) — Migration Reference

> Purpose of this document: describe the existing legacy WinForms (.NET Framework 4.8) application
> — its forms, business workflows, data model (DTOs), external API usage, hardware integrations, and
> configuration — so that another engineer/agent can **rewrite** the same scenarios using a different
> technology stack. This document only **describes** the current system; it does not propose a new
> architecture or implementation.

## 1. Application overview

- **Name:** RfidConnectRegister (solution/README title: "Silo Windows Register"), company "آویژه" (Avizhe).
- **Platform:** Windows Forms, C#, .NET Framework 4.8 (single project `RfidConnectRegister.csproj`).
- **Domain:** Factory/warehouse product "register & label print" system using RFID (UHF) tags and
  barcodes. Operators register production batches (serials), print physical labels (barcode / RFID tag
  encode / ZPL), pair barcode+RFID EPC, cancel/correct registrations, group items into parent
  packages, and run analytical/report screens. Reference to pipe/coupling (oil & gas) and ceramic tile
  products appears in the API/business logic, so the app has been reused across more than one factory's
  product catalog (generic "Product" model with technical/dynamic fields per product type).
- **Architecture style:** Thin client WinForms app calling a remote "RfidCore"/WMS HTTP API using a
  single RPC-style endpoint (method name dispatch, not REST resources). Local SQL Server access also
  exists via a generic ADO.NET helper, but most CRUD/reporting flows go through the HTTP API
  (`ApiBusiness`), not direct SQL, in current code.
- **No automated tests** exist in the repository.

### Solution/project layout
```
RfidConnectRegister/
  BussinessLogic/
    AppRegister/        - node-locked license/activation
    Classes/            - helpers: Excel I/O, RFID reader drivers, settings, Shamsi calendar, logging, COM port discovery
    RfidConnectApi/      - HTTP API clients (3 generations: RestAPI/Connector, ApiBusiness/WorkingWithApiConnector)
    (root files)         - ZPL label printing, SMS, notification polling, tray app, date/time helpers
  DML/                 - DTOs/models (plain C# classes + a few enums)
  DataAccess/           - generic ADO.NET SQL executor + static "current session" globals
  Properties/           - app Settings (user-scope, written to app.config), resources
  UI/                   - WinForms forms (Form1 demo + FrmLogin + formSetting + frmShowAlert)
  UI/StoreProduct/      - the actual business forms (register, print, reports, master data, QA)
  Program.cs            - entry point
  app.config            - settings + Newtonsoft.Json binding redirect
  packages.config        - NuGet dependencies (old-style packages.config, not PackageReference)
```

## 2. Configuration (app.config / Properties.Settings)

All settings live under `RfidConnectRegister.Properties.Settings` (user-scope `ClientSettingsSection`),
read/written both via strongly typed `Properties.Settings.Default.*` and a parallel free-form store
(`GetSettingDictionary.DicSetting`, backed by a `ApplicationConfig.log` file at the exe's startup path,
key=value pairs separated by `/`). Keys are centralized as constants in `BussinessLogic/Classes/ConfigKeys.cs`.

Key settings (grouped):

- **Server/connectivity:** `ServerIP` (base host:port used to build the HTTP API URL), `RfidConnectServerIp`
  (default `localhost:97`), `ConnectionString` (SQL Server connection string, empty placeholder filled at
  install time), `api_key` (SMS panel key, usually blank — real key hardcoded in `sms.cs`).
- **Device identity:** `DeviceID` (default `2`), `DeviceIP`.
- **RFID/barcode reader hardware:** `TableModel` (default `P1`), `ReaderPower` (default `7`), `PortNumber`
  (default `COM9`), `COMPairDevice` (e.g. `Device1=COM4/`), `DeskReaderType` (default `1`), `DeskPower`
  (default `0`), `DeskUHFReader_Port`, `SerialLength` (default `5`), `BarcodeLength`/`BarcodeLength1`/
  `BarcodeLength2`, `BarcodeIndex`, `BarcodeSpliter`, `BarcodeType`, `IgnoreEpcList` (comma-separated EPCs
  to ignore, e.g. test/empty tags).
- **Printing/labels:** `PrinterModel`, `PrinterInOperation`, `PrintFileAdress` (default
  `reports\roka_Tag.frx` — FastReport template path), `KindPrint` (default `A5`), `IsZpl` (ZPL raw-printing
  mode vs FastReport/ZMIN mode), `MergPrintToPDF`, `PrinterModelForAutoPrintAfterRegister` (default
  `Canon MF3010`), `AutoPrintAfterRegister`, `PrintPreviewBeforePrint`, `PrintPaperType`,
  `ControlPrintFormatByBrand` (filter which label template appears per product brand), `ShowSaveDialog`.
- **SMS:** `SmsUserName`, `SmsPassword`, `SmsNumber1`, `PanelType` (all typically blank; real
  credentials/endpoint are hardcoded in `sms.cs`).
- **Business-rule flags (feature toggles):** `KindRegister` (default `1`; a mode selector — value `3`
  triggers immediate registration during "print only" flow), `HaveLogin` (enables username whitelist
  check before saving a new registration), `NotifApp` (enables tray/background notification mode after
  login instead of opening the main form directly), `MultiProductInOnePrintActionId` (allow several
  products under one print-action batch), `CheckRegisterByDocItems` (validate entered count against a
  production-document's remaining quantity), `CountInPackEqualCount`, `ComputeAverageProductPackValueFromAllWeight`
  (derive pack value from total weight), `AllowShowOnlyPrintFormBeforeSavePrintAction`,
  `RegisterCountEvryTimeIsOne`, `ActiveRegisterFormByOneSerial` (lock the register-by-barcode form to a
  single expected serial), `SetParentForSerials` (enable parent-package assignment workflow),
  `AutoDetecteShift` (auto-compute production shift from current time), `GetListSerial`.
- **Company/branding/version:** `CompanyName` (default `آویژه`), `SoftwareVersion` (default `1.0.0`).
- **Runtime:** targets `.NETFramework,Version=v4.8`; `Newtonsoft.Json` binding redirect to 13.0.0.0.

### NuGet dependencies (packages.config)
- `Microsoft.Office.Interop.Excel` 15.0.4795.1000 — Excel COM interop (used for some export/import flows).
- `Newtonsoft.Json` 13.0.1 — JSON serialization for the HTTP API layer.
- `PDFsharp` 1.50.5147 — merging per-row exported PDFs into a single PDF (`MergPrintToPDF` feature).
- `Syncfusion.Compression.Base`, `Syncfusion.Licensing`, `Syncfusion.Pdf.WinForms` 20.4.0.44 — commercial PDF
  toolkit (licensed component).
- Also referenced in code (not all necessarily in packages.config): **FastReport** (`.frx` template
  rendering/printing used across report & label forms) and a vendor **"LabelPrinter" SDK**
  (`ZMPrinter`/`ZMLabel`/`PrintUtility`/`LSFUtility`) for RFID label printer hardware, plus `RfidApiLib`
  (UHF desk reader driver SDK).

## 3. Data model (DML — DTOs)

All plain C# POCOs under `RfidConnectRegister.DML`. These are the shapes the UI and API layer exchange
data in; they are **not** EF entities (no attributes/mapping) and some clearly describe legacy/renamed
database columns via comments.

- **`Document` (Document.cs):**
  - `DocumentHeader`: Id, Key, UserId, ImportType (enum: Excel/Api/Manual/Other=1234), FileName,
    DocumentType (int), ImportDateTime, Description, Status, HeaderData (raw string, likely JSON),
    DocumentItems (collection).
  - `DocumentItem`: Id, Key, DocumentType, ProductCode, ProductTitle, Count, Remain, ProductUnit,
    ItemData (raw string), StatusDesc.
  - `GetAllAggDocDto`: DocumentKey, DocumentType, ImportDateTime?, ItemCount, ItemSum (decimal),
    DocumentData. Used for an aggregated/summary view of documents ("production plan" documents).
- **`DynamicFieldDto` (DynamicFieldDto.cs):** server-defined custom/technical fields attached to products
  or registration actions. Fields: Id, Title, FieldType (enum `ItemData`/`HeaderData`), IsSystematicField,
  IsHeaderKey, UserName, UserId, DateTime, RelatedTitle1-3, ActionType?, FieldShowColumn,
  FieldShowColumnForAction, ValueType (enum `TextBox`/`DropDown`/`RichTextEditor`/`Numeric`), ValueOptions
  (e.g. serialized options for dropdown), DefaultValue, IsRequired?, SectionId?, Order?, ParentId?, Value.
  Drives dynamically generated UI panels for arbitrary per-product "technical data" fields (JSON stored on
  the product/tag record and parsed back into these DTOs at runtime).
- **`EnterRequest` (EnterRequest.cs):** ProductCode, ProductName, FromDate/ToDate, FromTime/ToTime,
  TechnicalCode, Shift, ProductSerial — a simple search/filter request shape (string-typed dates/times,
  no DateTime).
- **`GetRegisterRequestFilter` (GetRegisterRequestFilter.cs):** the main filter DTO for the register-tag
  report screen: ProductCode, Line, FromDate/ToDate, User, Shift, Qc, TechnicalCode, ProductSerial,
  ProductOldSerial, TechnicalCodeLike (bool), Size (default `"-1"`), ContractStatus, ProductGroup,
  ProductBrand, RegisterDevice (default `"-1"`), InspectStatus, FromTime/ToTime.
- **`MovementAction` (MovementAction.cs):** MovementActionId, MovementActionUserId, MovementActionDate/Time,
  MovementActionCountTags, MovementActionCarPlaque, MovementActionDriverName, MovementActionDriverMobile,
  MovementActionData, MovementActionUHFLogIDData, MovementActionDesc, MovementActionDocumentId — models a
  shipment/transport event for a batch of tags (loading dock scenario), all string-typed fields.
- **`Print` (Print.cs):** the richest DTO — one row = one queued/printed/registered label item. Fields
  include Id, ProductSerial, ProductOldSerial, ProductTitle/ENTitle, ProductTechnicalData (raw JSON),
  RegisterDate, RegisterUser, ProductCode, ProductDescription, ProductCount (decimal), TagEpc, PrintFlag,
  RegisterFlag, RegisterType, InputFileName, PrintType, PrintUser, ErrorTime/ErrorDesc, PrintActionId
  (groups rows into one print batch), PrintActionDateTime, ProductUnit, ProductItemCount,
  ProductCountInPack, ProductSize/SizeTitle, ProductRegCode, ProductWeight/Volume (decimal),
  ProductStatus/StatusTitle, ProductBrand/BrandTitle, ProductGroup/GroupTitle, ProjectCode,
  ProductProductionShift/ShiftTitle, ProductProductionLine/LineTitle, ProductContractType, ProductType/
  TypeTitle, DocumentId, WareHouseCode, ProductProperties (raw JSON of dynamic fields).
- **`Product` (Product.cs):** master-data product record — ProductCode, ProductTitle/ENTitle,
  ProductPackValue, ProductCountInPack, ProductValue, ProductPackWeight/Volume, ProductTechnicalCode,
  ProductProperties (raw JSON), ProductType, ProductStatus, ProductSize/SizeTitle, ProductUnit,
  ProductBrand/Group (+ titles), ProductTechnicalData (raw JSON), ProductRegUser. (All numeric-looking
  fields are strings in this DTO — the UI formats/parses them.)
- **Lookup/reference DTOs (simple code+title pairs):** `ProductBrand`, `ProductGroup`, `ProductSize`,
  `ProductStatus`, `ProductType` (each: `<X>Code`, `<X>Title`), `ProductPropertyC`
  (ProductPropertyC_Id/Title — appears to represent product "size" category in some forms),
  `ProductionLine` (maps to DB table `tbl_ProductPropertyA`: ProductionLineCode/Title ≈
  `fld_ProductPropertyAId/Title`), `ProductionShift` (maps to `tbl_ProductPropertyB`:
  ProductionShiftCode/Title ≈ `fld_ProductPropertyBId/Title`).
- **`Tag` (Tag.cs):** the core "registered RFID tag / item" record, combining product + registration +
  tree/parent-package + movement/action history data. Notable fields: ProductSerial/OldSerial/Code,
  TagEpc, ProjectCode, ProductCount, ProductName, ProductType (int)/Title, ProductStatus/Title,
  TagStatus (int)/Title, TagRegisterShamsiUnixDate, TagRegisterUser, TagRegisterDateTime,
  TagTreeParentId/SecondParentId/ParentsId (package hierarchy), NewProductSerial, ProductProperties (raw
  JSON), Lock/Freeze/Deactivate (bool flags), TagInActionId/DestinationId + a second pair
  (`...2`) (current/previous warehouse-movement state), `fld_ProductPropertyA/B/CId` + titles (line/shift/
  size-like custom properties), RegCode, ContractStatus, TagZone, ProductUnit, FreezeStatusTitle,
  InspectStatusTitle, ProductValue/PackWeight/PackVolume/CountInPack, ProductSize, TagTreeParentSerial/
  ParentsEpc, RActionId/Date/Time (last action), HActionId/Date/Time/Data +
  HMovementActionCarPlaque/DriverName (last "H" movement/shipment action). This is effectively the union
  of "item", "tag/RFID state", "package membership" and "last movement" concerns in one flat DTO.
- **`User` (User.cs):** UserName, UserId, UserType.
- **`WarehouseDto` (WarehouseDto.cs):** Id, DestinationCode, DestinationTitle,
  OperationalType (enum `DestinationOperationalType`: NotSpecified=-1, Production, Product, Material,
  Waste, Loading), InventoryType (enum `DestinationInventoryType`: NotSpecified=-1, Virtual, Physical),
  IsDefault, allowCancel (permission flag used by the cancel-registration form), allowRePrint, IsActive,
  Epc. Represents a warehouse/destination/zone a tag can be moved into.
- **`Zones` (Zones.cs):** ZoneCode/Title, ZoneCapacity, ZoneParentCode, ZoneStoreCode, ZoneParentLayer,
  MinZoneCapacity/MaxZoneCapacity — physical storage zone hierarchy/capacity (referenced but not clearly
  wired into the explored forms — likely used by other modules/screens not present or by server-side
  logic only).

## 4. External API layer (`BussinessLogic/RfidConnectApi`)

Three generations of HTTP client code coexist:

1. **`RestAPI.cs` (class `RestApi`) — legacy RPC client.**
   - Endpoint: `http://{RestApi.Ip}/RfidConnectApi/RfidConnectApiHandler.ashx` (single fixed URL; `Ip` is a
     static field set at startup/login from `Properties.Settings.Default.ServerIP`).
   - Protocol: single POST per call; JSON envelope `{ "interface": "RestAPI", "method": "<MethodName>",
     "parameters": { ... } }`; response parsed as a `JObject`. No authentication header (method-name
     dispatch style "RPC over HTTP").
   - Representative methods: `Receipt`, `PrintDelete`, `PrintSelectData`, `PrintUpdatePrintStatus`,
     `PrintInsertUpdate`, `GetModelDevices`, `GetModelNames`, `Login`, `GetProductInfo`, `ReadedTags`,
     `GetShamsiUnixDateTime`, `SaveReadedTag`, `ASsignPipeToCoupling`, `Reka`, `CheckingPipe`,
     `CheckingPipeWithCoupling`, `AssignRfidTagToProduct`, `IsBarCodeExist`, `IsTagEpcExist`,
     `HasOnlinePrintData`, `SaveServiceLog`, `PrintCancel`, `IsTagAssignedToProductSerial`,
     `UpdateEpcByProductSerialAndGetProductInfo`, `ReregisterRequest`. (Domain hints: pipe/coupling
     manufacturing QC — this app's API surface has served more than one product line historically.)

2. **`Connector.cs` (internal `Connector`) — generic HTTP transport helper.**
   - Static async/sync methods (`ConnectAsync`, `ConnectByMethodAsync`, etc.) that POST/`JsonConvert`-
     serialize an arbitrary object to a caller-supplied URL with `Content-Type: application/json`, default
     60s timeout. No fixed URL or auth baked in — pure transport primitive used by the next layer.

3. **`WorkingWithApiConnector.cs` (`WorkingWithApiConnector`) + `ApiBusiness.cs` (`ApiBusiness`) — current
   production client, used by essentially all business forms.**
   - Base URL: `http://{Properties.Settings.Default.ServerIP}/RfidCore/v1/Wms/PostObject` — again a single
     RPC endpoint; the actual operation is selected by passing a method name + `KeyValuePair<string,
     object>[]` parameters (`WorkingWithApiConnector.LoadUnDirect`/`LoadUnDirectAsync`), built into a
     JSON body and POSTed. Response is a `JToken` with a `successful` boolean and a `value` payload.
   - `LoadDirect`/`LoadDirectAsync` variants exist for posting a raw string/object body to an arbitrary URL
     (optionally with a bearer token) — used for calls that don't fit the method-name/KVP pattern.
   - **Authentication:** `ApiBusiness.Login(username, password)` calls method `TSLogin`; on success the
     app fetches the display name via `TSLoginReturnName` and stores `DataAccess.Items.OnlineUserCode` /
     `OnlineUserName` / `OnlineUserRealName` as ambient "current session" state (no token/session is
     persisted — most subsequent calls just resend the username as a parameter where needed; some API
     calls support an optional bearer token at the transport layer but it is not used as a session scheme
     here).
   - **Business operations available (method names dispatched through `PostObject`), grouped by concern:**
     - *Auth/users:* `TSLogin`, `TSLoginReturnName`, `GetAllUser`.
     - *Documents (production plan):* `SGetDocumentsByDocType` (→ `GetAggDocsByDocTypeAndStatus`),
       `SGetDocumentData` (→ `GetDocumentData`), `SGetListOfRemainProductByDocumentId`.
     - *Product master data:* `GetAllProduct`, `GetAllWMSProduct` (external WMS catalog lookup for cloning
       base product data), `GetAllProductByTechnicalData`, `SaveProduct`, `DeleteProduct`,
       `GetAllProductStatus`, `GetAllProductType`, `SaveProductType`, `GetAllProductPropertyC`.
     - *Warehouses / dynamic fields:* `GetAllWarehouses`, `GetDynamicFieldsByActionTypeId`,
       `GetDynamicField`.
     - *Serial/tag/registration workflow:* `CheckProductSerial`, `IsProductSerialExist`, `CheckTagEPC`,
       `CreateEpc`, `GetTagsInfo` (bulk), `GetTagInfo` (single), `GetTagListByParentCode`, `GetPrintInfo`,
       `SaveRegisterTag`, `CancelRegisterTag`, `ChangeRegisterDate`, `SRegisterNextlevelProductUnit`
       (assign/detach child serials to/from a parent package), `GetLatestNextlevelProductUnitbyEpc`.
     - *Reporting:* `RepRegisterTag` (detail), `RepSCancelRegisterTag`, `RepSGetNotRegisteredPrints`,
       `RepRegisterTagByTagStatus0`, `SRepRegisterTagWithOutEPC`, `RepRegisterTagTajamoeeProductCode`
       (aggregate), `RepRegisterTagSummaryGroupBySizeANDRegCode` (pivot/cross-tab), notifications poll via
       `SGetNotif_WinFormService`.
     - *Printing:* `SavePrintBySerial`, `GetNewPrintActionId`, `GetPrintDataByByProductSerial`,
       `GetPrintListByPrintActionId`, `PrintUpdatePrintStatus`, `SDeletePrintByPrintId`.
     - *Production master data:* `GetAllProductionLine`, `GetAllProductionShift`.

> Migration note for the target system: since there is no REST resource modeling on the existing API —
> every operation is a named RPC call through one POST endpoint — the new implementation needs to either
> replicate this RPC contract against the same backend, or (if the backend is also being rewritten)
> re-derive proper REST/resource semantics from the method list and DTOs above.

## 5. Local database access (`DataAccess`)

- **Technology:** Plain ADO.NET (`System.Data.SqlClient`), no ORM. `DataAccess` class takes a connection
  string (sourced from `Properties.Settings.Default.ConnectionString`) and exposes four generic
  operations: `CmdSqlExecuteNonQuery`, `CmdSqlExecuteNonQueryWithTransaction` (multiple commands in one
  `SqlTransaction`), `SqlDataAdapter` (returns `DataTable`), `SqlDataReaders` (returns `SqlParameter[]`
  from the first row). No named repository methods or hardcoded table/proc names exist in this class —
  callers build arbitrary `SqlCommand`s. In the explored UI forms, this local DB path does not appear to
  be the primary data source (the HTTP API is); it may be a legacy/alternate path or used in parts of the
  app not covered by this document.
- **`Items.cs`:** static in-memory "current session" globals set after a successful `ApiBusiness.Login`
  — `OnlineUserCode`, `OnlineUserName`, `OnlineUserRealName`, plus `NewCustomerCode`, `BranchCode`,
  `BranchName` (used across forms as ambient current-user/branch context, not persisted between runs).

## 6. Licensing/activation (`BussinessLogic/AppRegister/RegisteryClass.cs`)

Node-locked license check performed (commented out in the current `Program.cs`, so effectively
**disabled** in the latest build, but present in code):
- `GetCPUId()` reads `Win32_Processor.ProcessorId` via WMI as a hardware fingerprint.
- `CreateActiveCode()` derives a deterministic activation code from CPU-ID characters + a hardcoded seed
  (`"356017"`) via arithmetic + hex conversion.
- `CheckRegistery()` compares a value stored under `HKCU\SOFTWARE\TDAvizhe\RFIDConnect_antenna\
  RFIDConnectOfVars` against the freshly computed code; returns 1 (valid) or 3 (invalid/unlicensed). The
  registry value itself is written by an external installer/activation step not present in this repo.

## 7. Hardware/device integration

- **UHF RFID desk reader:** `BllConectToDeskReader`/`BLLConnectToUHFReader` (wraps vendor SDK
  `RfidApiLib`), supports multiple reader models (`ConnectToDeskUHFReader`, `ConnectToTinyUHFReader`,
  `ConnectToT64UHFReader`) and a dedicated `ConnectTo6061.cs` wrapper for one specific reader model.
  Configured via `ConfigKeys`/`Properties.Settings` (reader port, power, model). Readers push detected
  EPCs into bound `TextBox` controls (e.g., `txtEpc`), with timers to reconnect, clear stale reads, and
  support two modes: single/continuous identify (mode `1`) and group/bulk identify (mode `2`, used by the
  batch QA scan form).
- **Barcode scanners:** treated as keyboard-wedge input into regular `TextBox` controls (e.g.
  `txtSerialCode`), parsed using configured splitter/index/length rules (`ConfigKeys.BarcodeSpliter`, etc.)
  rather than a dedicated barcode-scanning API.
- **COM port discovery:** `GetComPort.cs` enumerates serial ports via WMI (`Win32_PnPEntity`) for the
  settings screen.
- **Label printing — three mechanisms, selectable per settings/template:**
  1. **FastReport** `.frx` templates (default) — used by nearly all report/print forms; supports a "merge
     exported PDFs into one" post-processing step via PDFsharp (`MergPrintToPDF` setting), optional print
     preview, and per-brand template filtering.
  2. **Raw ZPL printing** (`ZPLHelper`/`ZPLHelperBase`) — used when `IsZpl` setting is enabled; builds raw
     ZPL commands (including embedding a label bitmap as a `~DG`/GRF image) and sends them directly to the
     printer driver via raw document printing (bypassing GDI), with text placeholders substituted from
     product/print record fields (serial, code, title, dates, technical data, etc.).
  3. **Vendor "LabelPrinter" SDK (ZMIN format)** — `ZMPrinter`/`ZMLabel`/`PrintUtility`/`LSFUtility`
     classes compose/print labels and can simultaneously **encode an RFID tag** while printing (single
     combined "print + write EPC" operation), with printer-status codes mapped to user-facing
     Persian/English error messages (cutter, ribbon, paper-out, RFID read/write failures, firmware
     update codes).
- **Excel I/O:** `AvizheExcel.cs` reads `.xls`/`.xlsx` via OLE DB (`Microsoft.Jet.OLEDB.4.0`) for importing
  serial lists / product data, and also supports exporting a grid's `DataTable` to Excel
  (`AvizheExcel.ExcelExport.ExportDatatable`), used by report screens' "Export" buttons.
- **SMS:** `sms.cs` (`SmsRequest`) sends SMS via HTTP GET/POST to `https://avizhegroup.com/sms/SendSms.php`
  with hardcoded credentials/API key and a hardcoded per-"SMSKind" phone-number distribution list (1–4).
  `SchasualedSmsCreator.cs` (class `SchasualedNotifCreator`) is actually a **notification poller**, not an
  SMS sender: a timer (30s) calls `ApiBusiness.SGetNotif_WinFormService(lastCheck)` and shows a toast-like
  popup (`frmShowAlert`) when the server has pending alert text.
- **System tray mode:** `TaskTrayApplicationContext.cs` runs the app minimized to a `NotifyIcon` with a
  context menu ("notification report" / "logout") — activated after login when `NotifApp` setting is on,
  combined with the notification poller above.

## 8. Calendar/date handling

- `ShamsiCalendar.cs` and `BussinessLogic/Time.cs` provide Persian (Shamsi/Jalali) ↔ Gregorian date
  conversions (`System.Globalization.PersianCalendar`), plus a "Unix Persian time" string format
  (`yyyyMMddHHmmss`) used as a timestamp/version key in several API calls (e.g. notification polling,
  report filters "last day" queries). Most date fields across DTOs are plain strings (not `DateTime`),
  formatted/parsed ad hoc by each form.

## 9. Forms — business scenarios, one section per form

All business forms live in `UI/` (shared/login/settings) and `UI/StoreProduct/` (core workflows). Unless
noted, forms call the server exclusively through `ApiBusiness` and use `DML.*` DTOs to shape data.

### 9.1 `FrmLogin` (UI/FrmLogin.cs) — application entry/login
- Sets `RestApi.Ip` from the configured `ServerIP` at startup.
- Authenticates via `ApiBusiness.Login(username, password)`; on failure shows a warning and/or reveals an
  inline server-IP configuration panel if connectivity failed (`WebException`/`AggregateException`).
- On success, stores `DataAccess.Items.OnlineUserName`, then either (a) enters **tray/background
  notification mode** (`TaskTrayApplicationContext` + `SchasualedNotifCreator`) if `NotifApp` is enabled,
  or (b) opens `frmStoreRegisterTags` directly.
- Lets the user persist a new server IP (writes to `GetSettingDictionary` + `Properties.Settings`).
- Navigates to: `frmStoreRegisterTags` (success), `FormSetting` (gear icon).

### 9.2 `FormSetting` (UI/formSetting.cs) — configuration screen
- Single screen to configure: installed printer + print-in-operation flag + save-dialog behavior; barcode
  decoding rules (what the barcode represents: product id/code/serial; splitter; index; lengths); reader/
  device settings (model, COM port, power, ignore-EPC list, serial length — device-model-specific dynamic
  controls were planned but are currently commented out/incomplete); many boolean feature flags (merge-to-
  PDF, control-print-format-by-brand, auto-detect-shift, get-list-serial, allow-show-only-print-form-
  before-save, register-count-always-one, print-preview-before-print, active-register-by-one-serial,
  set-parent-for-serials, have-login, notif-app, is-ZPL); server IP + company name; a checkbox list of
  available `.frx` report files (single-select) to use as the active label template; register-mode and
  print-kind selectors.
- Saving persists every value into both `Properties.Settings` and `GetSettingDictionary`'s file-backed
  store, with basic numeric/non-empty validation on barcode splitter/index.
- Opened modally from `FrmLogin` and `frmStoreRegisterTags` (gear icons).

### 9.3 `frmShowAlert` (UI/frmShowAlert.cs) — toast notification popup
- Transient, always-on-top, bottom-right popup showing a message (plays a system sound), auto-closes on
  timer or click. Used by the scheduled notification poller / tray mode.

### 9.4 `frmStoreRegisterTags` (UI/StoreProduct/frmStoreRegisterTags.cs) — **main hub / register new batch**
This is the central production-registration screen and the app's workflow hub.
- **Product selection:** manual code entry or `frmSearchProducts` picker; auto-fills product metadata
  (title, EN title, technical code/reg code, type, unit, pack sizes/weights/volumes) from the product
  master list.
- **Production metadata entry:** production line, production shift (optionally auto-detected from current
  time via `AutoDetecteShift`), warehouse/store destination, product quality status, and a
  document/roll code tied to a production-plan "document" (optional document search panel showing
  permitted products and remaining quantities for that document).
- **Quantity rules:** count of serials to generate; optional validation that the entered count doesn't
  exceed the document's remaining allowed quantity (`CheckRegisterByDocItems` setting); a helper to
  compute pack-value from total weight (`ComputeAverageProductPackValueFromAllWeight`).
- **Serial list input methods:** auto-generate N serials by count; import a list of explicit serials from
  an Excel file; manually build/compute a serial list in a grid.
- **Multi-product batches:** optional mode (`MultiProductInOnePrintActionId`) to accumulate several
  products under one shared `PrintActionId` before saving them all together.
- **Dynamic/technical fields:** per selected product, loads server-defined `DynamicFieldDto` definitions
  and renders matching editable controls, parsing/serializing the product's technical-data JSON.
- **Access control:** if `HaveLogin` is enabled, only a hardcoded whitelist of usernames may save a new
  registration (simple role check, not server-enforced in this screen).
- **Save:** validates required fields, shows a confirmation summary, then calls
  `ApiBusiness.SavePrintBySerial` (single or looped per multi-product row) to create print/registration
  records (`DML.Print`), then offers to open the print/register screen for the newly created batch.
- **Navigation hub:** opens `frmDefineProducts`, `frmDefineProductType`, `frmSearchProducts`,
  `frmPrintRegsiterTags` (single-product print), `frmOnlyPrint` (multi-product print), `frmRepCancelRegister`,
  `frmRepErrorInRegisterTag`, `frmRepRegisterTag`, `frmCancelRegisterTag`, `frmTestRegister`/
  `frmTestRegisterGroup`, `frmChangeTagRegisterData`, `frmSetParent`, `FormSetting`.
- Shows a live clock and the current logged-in user; supports navigating to previous/next print actions.

### 9.5 `frmPrintRegsiterTags` (UI/StoreProduct/frmPrintRegsiterTags.cs) — **print & register labels (new batch)**
The main "print the labels for a batch just created, encoding/registering the RFID tag at the same time"
screen.
- Loads the queued print rows for a `PrintActionId` (`GetPrintListByPrintActionId`).
- Three label/printing paths, chosen by settings:
  1. **ZPL raw printing** — fills a ZPL template's text placeholders from the row's product/print data
     (serial, old serial, titles, dates, technical code, batch quantity, document code, tag id, plus any
     dynamic technical fields), registers the tag server-side (`SaveRegisterTag`), sends the ZPL to the
     printer, retries on failure.
  2. **ZMIN format via vendor "LabelPrinter" SDK** — loads a label definition file, maps the same
     placeholder set into the SDK's label variables, registers the tag, prints (RFID write path exists in
     the SDK but appears disabled/stubbed in current code), maps printer status codes to error messages.
  3. **FastReport templates** (same mechanism as `frmOnlyPrint`, see below) — for formats not requiring
     ZPL/ZMIN.
- Supports adding extra serials to the queue, editing/deleting a queued row (via a dynamic-fields edit
  panel, blocked once a row's tag is already registered), and merging multiple exported PDFs into one.
- Navigates from `frmStoreRegisterTags` right after saving a new (single-product) registration batch.

### 9.6 `frmOnlyPrint` (UI/StoreProduct/frmOnlyPrint.cs) — **print-only for an existing batch (multi-product variant)**
Same queue/edit/delete/dynamic-fields/FastReport mechanics as `frmPrintRegsiterTags`, but:
- No ZPL/ZMIN tag-encoding path is embedded here directly — RFID registration, when needed
  (`KindRegister == 3`), is delegated to `frmRFIDRegisterByBarfid`, or performed implicitly via
  `SaveRegister` using a derived EPC when printing via FastReport.
- Adds a "search not-registered serials" feature to bulk-add missing serials back into a print queue
  (`RepSGetNotRegisteredPrints`), and a WMS product lookup/import panel for reference.
- Opened from `frmStoreRegisterTags` when `MultiProductInOnePrintActionId` is enabled (alternative entry
  point to `frmPrintRegsiterTags`).

### 9.7 `frmRFIDRegisterByBarfid` (UI/StoreProduct/frmRFIDRegisterByBarfid.cs) — **pair barcode + RFID tag, register**
The core "bind a printed barcode label to a specific physical RFID tag" workflow.
- Reads a barcode (serial) from a scanner input box and an EPC from the desk RFID reader; once both are
  present within a time window, calls `SaveRegister`:
  - If the EPC is already bound to a different serial, asks for confirmation to reassign it.
  - Checks `IsProductSerialExist`: unknown/not-queued serial → error; queued-but-unregistered → registers
    (`SaveRegisterTag`); already-registered → asks to confirm overwrite with the new tag.
- Can run standalone (free mode, pairs any barcode+tag) or pinned to one pre-supplied expected serial
  (used when launched for a single just-printed label), in which case it auto-closes with
  `DialogResult.OK` after a successful pairing.
- Provides shortcuts to `frmCancelRegisterTag` and `frmTestRegister`.
- Opened (modally) from `frmOnlyPrint`/`frmPrintRegsiterTags`/`frmStoreRegisterTags`.

### 9.8 `frmCancelRegisterTag` (UI/StoreProduct/frmCancelRegisterTag.cs) — **void/cancel a registered tag**
- Operator scans a tag on the desk reader; the form loads the tag's current info (`GetTagInfo`) and
  determines whether cancellation is permitted for the tag's current warehouse/destination
  (`WarehouseDto.allowCancel`, via `GetAllWarehouses`).
- If permitted, `CancelRegisterTag(epc, user)` voids the registration; shows success/failure feedback and
  resets for the next scan.

### 9.9 `frmChangeTagRegisterData` (UI/StoreProduct/frmChangeTagRegisterData.cs) — **correct registration date/time**
- Scans a tag, loads its info; edit is only allowed if the item has not yet been inspected
  ("بازرسی نشده") **and** is still within production warehouses ("انبارهای تولید"); otherwise the form is
  locked (visually flagged) to prevent changes.
- On save, validates the new date/time format and calls `ChangeRegisterDate(serial, date, time)`.

### 9.10 `frmSetParent` (UI/StoreProduct/frmSetParent.cs) — **group items into a parent package**
- Add child item serials by barcode scan or manual entry, or scan a package's own RFID tag (parent EPC);
  shows whether a child item already belongs to another package.
- Loads existing children of a package by parent serial/EPC (`GetTagListByParentCode`).
- Saves the grouping via `SRegisterNextlevelProductUnit(parentEpc, serialList, user)`; retrieves/display
  the generated package id; supports removing a child from a package (same API with `"-1"` as parent).
- Prints a package-summary label (serial list, item count, total value) via FastReport.

### 9.11 `frmTestRegister` (UI/StoreProduct/frmTestRegister.cs) — **QA: verify barcode ↔ tag correspondence**
- Reads the barcode serial (`GetPrintInfo`) and the RFID tag's registered serial (`GetTagInfo`)
  independently, then compares them, flagging a correct match (green) vs. mismatch — a tag registered for
  a different product than what's printed on the label (red) — or an unregistered tag.

### 9.12 `frmTestRegisterGroup` (UI/StoreProduct/frmTestRegisterGroup.cs) — **QA: bulk/continuous tag scan audit**
- Continuously reads tags from the desk reader in group/bulk mode, deduplicating by EPC; periodically
  batch-checks all newly seen EPCs (`GetTagsInfo`) and lists each as registered (green, with its product
  info) or unregistered (red), with running totals — used to audit a shipment/pallet's tags quickly.

### 9.13 `frmDefineProductType` (UI/StoreProduct/frmDefineProductType.cs) — **master data: product types**
- Simple CRUD-style screen (grid + code/title fields) backed by `GetAllProductType`/`SaveProductType`.

### 9.14 `frmDefineProducts` (UI/StoreProduct/frmDefineProducts.cs) — **master data: products**
- Full product CRUD: code, titles (local/EN), technical code, unit, value, pack count/value/weight/volume,
  size, type, status — backed by `SaveProduct`/`DeleteProduct`/`GetAllProduct` + several lookup lists
  (`GetAllProductStatus`, `GetAllProductType`, `GetAllProductPropertyC`).
- Includes a hardcoded business rule mapping certain size strings (e.g. "75*75", "60*60", "60*120" — tile
  dimensions) to default value/count-in-pack/pack-value figures.
- Secondary panel to search an external WMS product catalog (`GetAllWMSProduct`) and copy code/title into
  the new-product form (import/clone starting point).

### 9.15 `frmSearchProducts` (UI/StoreProduct/frmSearchProducts.cs) — **reusable product picker dialog**
- Modal, multi-criteria client-side filter (code/title/size/status/reg code/group/brand/type/registration
  document) over an in-memory product list (passed in by the caller or reloaded via `GetAllProduct`);
  selecting a row writes the `ProductCode` back into the caller's textbox and closes with `DialogResult.OK`.
- Reused by `frmDefineProducts`, `frmOnlyPrint`, `frmPrintRegsiterTags`, `frmRepRegisterTag`,
  `frmStoreRegisterTags`.

### 9.16 `frmRepCancelRegister` (UI/StoreProduct/frmRepCancelRegister.cs) — **report: cancelled registrations**
- Filter by serial/date range/user (`RepSCancelRegisterTag`), results grid, Excel export.

### 9.17 `frmRepErrorInRegisterTag` (UI/StoreProduct/frmRepErrorInRegisterTag.cs) — **report: problematic registrations**
- Auto-run on load, two lists: items registered with `TagStatus==0` in the last day
  (`RepRegisterTagByTagStatus0`), and items registered without any EPC assigned (`SRepRegisterTagWithOutEPC`).

### 9.18 `frmRepRegisterTag` (UI/StoreProduct/frmRepRegisterTag.cs) — **main analytics/report screen**
- Rich multi-criteria filter (`GetRegisterRequestFilter`: date/time range, line, shift, user, technical
  code, status, serial/old serial, size, document) requiring a production line selection.
- Three result views: aggregate summary by product code (`RepRegisterTagTajamoeeProductCode`), a pivoted
  cross-tab by size/reg-code with computed column sums and a totals row (`RepRegisterTagSummaryGroupBySizeANDRegCode`),
  and a detail grid (`RepRegisterTag`); clicking a summary/pivot row drills into matching detail rows.
- Excel export of the active tab; printed cross-tab report via FastReport with a human-readable filter
  description header.

### 9.19 Non-business reference form
- `Form1.cs` (namespace `WindowsFormsApp1`, not `RfidConnectRegister`) is a leftover vendor-SDK demo for
  the "LabelPrinter" RFID label SDK (print + write EPC in one action, with printer status-code handling).
  It is **not part of the real application navigation/workflow** — informative only as a reference for how
  the ZMIN label printing path (also used inside `frmPrintRegsiterTags`) works.

## 10. Cross-cutting business rules worth preserving in a rewrite

- Barcode content is decomposed using configurable rules (splitter character, segment index, expected
  length) rather than a fixed format — the target system should keep this configurable-parsing approach
  or explicitly document a fixed barcode format decision.
- A tag/serial can be in one of several lifecycle states: queued (printed, not registered) → registered →
  (optionally) cancelled/voided; also inspected/not-inspected and locked/frozen/deactivated flags; date-of-
  registration can be corrected only before inspection and only while still in a production warehouse.
- EPC reassignment (overwriting which serial a physical tag is bound to) is allowed but always requires
  explicit operator confirmation.
- Parent/child packaging is modeled via parent EPC/serial references on the child `Tag` records
  (`TagTreeParentId`/`TagTreeParentsId`), not a separate join table exposed to the client.
- Many numeric-looking DTO fields are transmitted as strings; dates are frequently strings in
  Shamsi-related or ad hoc formats rather than real `DateTime` — the rewrite should decide explicitly
  whether to preserve string-based date handling for compatibility with the existing backend/API or
  switch to native types (requires backend coordination since the API contract is shared).
- "Dynamic fields" allow the server to define extra technical attributes per product/action without
  client code changes; the UI renders them generically from `DynamicFieldDto` definitions — this
  extensibility mechanism should be preserved to avoid locking the rewrite to the current fixed field set.
- Feature-flag-driven behavior (see §2) significantly changes screen behavior (e.g., single vs multi-
  product batches, ZPL vs FastReport vs ZMIN printing, login-gated saves) — these flags represent distinct
  deployment configurations across different customers/factories and should all be accounted for, even if
  only a subset is used by any single deployment.
