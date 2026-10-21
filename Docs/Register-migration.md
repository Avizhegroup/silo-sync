# RfidConnectRegister ("Silo Windows Register") — Full Migration Specification

> **Audience:** an AI coding agent (or engineer) that must **rewrite** this application in another technology while keeping **the same scenarios**.
> **Nature of this document:** a *description of the existing system only*, derived from the legacy source (`SiloRegister-old`, project `RfidConnectRegister`, C# WinForms, .NET Framework 4.8). It contains **no new design, no proposed architecture and no invented behaviour**. Where the legacy code is buggy, ambiguous or incomplete, that is stated explicitly (**⚠ Defect** / **? Unknown**) so the rewrite can decide consciously; nothing is silently "fixed".
> **Supersedes:** the high-level `Docs/Register-migration.md` (silo-sync repo). Its overview is kept and corrected (it contained several inaccuracies — listed in §12).
> **Language note:** the UI is Persian (RTL). Persian labels are kept verbatim with an English gloss because several are used as **data keys** (report-template parameters, column-name mapping, string comparisons against server values) and must not be translated.

## How to read this document

| Part | Content |
|---|---|
| §1 | What the product does, runtime, project layout |
| §2 | Screen map, navigation, operating modes (`KindRegister`) |
| §3 | Configuration: every setting, defaults, where it is stored and used |
| §4 | The server API contract (single RPC endpoint): transport, **every RPC with its parameters and result fields** |
| §5 | Data model (DTOs) and the server-JSON → DTO field mapping |
| §6 | Domain glossary and terminology traps |
| §7 | **Per-form specifications F1–F18** (controls, load sequence, events, validations, exact messages, API calls, rules, gotchas) |
| §8 | Support code and hardware (transport, settings stores, RFID reader, scanners, notifications, dates) |
| §9 | Printing system (three mechanisms, templates, file conventions, parameter/token catalogs) |
| §10 | Business rules and state model (cross-cutting) |
| §11 | Consolidated list of hardcoded values and defects |
| §12 | Corrections to the earlier overview, and open questions the client code cannot answer |
| §13 | Appendix: helper functions, shipped files, dependencies, licensing, local DB |

Conventions: *"RPC"* = a named call to the single HTTP endpoint (§4). *"Hub"* = `frmStoreRegisterTags`. *"Print row"* = one `Print` DTO / one serial awaiting print/registration. *"Action id"* = `PrintActionId`, the batch id that groups print rows. Gloss in parentheses after Persian text.

---

## 0. Migration status

Target split: **Blazor** `Silo.Modules.Register` (web workflows) and **Windows** `Silo.Windows.Register` (RFID reader, scanner, printer). Status below reflects the code as of this update; "built" means the project compiles, not that it was tested against real devices or a running server.

| Form | Target | Status | Where / notes |
|---|---|---|---|
| F1 Login | Blazor host | Covered | Existing Silo login; no work needed. |
| F2 Batch definition (hub) | Blazor | Done except the items listed under "Remaining work" (not compiled, not run) | `/register/batch` (`MultiProductBatch`). **Two modes** (checkbox, locked once rows exist): *multi-product* (add rows, then Confirm → one `SSavePrintBySerial` per row) and *single product* (Confirm saves directly). **Serial list** (single mode): by range (legacy rule: end excluded, optional fixed prefix cut from the start value, counter padded to the start length) or from an `.xlsx` (first column, optional header row, max 5 MB / 50,000 serials); serials are validated (letters, digits, `_-./`) and de-duplicated, then saved one request per serial (`PrintCount` 1, `ProductSerial` = `ProductOldSerial`), awaited and **resumable** after a failure (legacy fired them without awaiting). Without a list the server generates the serials from the count. **Document panel:** list (`SGetDocumentsByDocType`, newest 300, filter by key) or typed key → `SGetDocumentData`; header JSON is parsed as JSON (legacy split on `:`) and shown, and copied into empty dynamic fields of the same title; items are merged per product and show requested/remaining (`SGetRemainRegisterByDoc`, minus unsaved rows); picking an item loads the product (`SPSearchProductWeb`) and sets the count to the remaining quantity converted to tags; with a document loaded only its products are accepted. **Remaining check** (checkbox, default on): requested = tags × (pack value ÷ product value), the same quantity the server subtracts; the legacy settings `CheckRegisterByDocItems`/`CountInPackEqualCount` are replaced by this single rule. Line/shift/store with one item are auto-selected; shift follows the clock when its title contains 1/2/3. After saving, a link opens `/register/print?actionId=…`. `Location` is now sent as "0" like legacy. **Also:** open an earlier operation by code or with previous/next, and the pack-value-from-total-weight helper; **user whitelist** (legacy `HaveLogin` with 12 hard-coded names): setting `Register:BatchAllowedUsers` (a list of user names in `appsettings.json`; empty = everybody with access to the page), checked in the page before adding/saving (a client-side check like legacy, the server does not enforce it); **keyboard shortcuts** F5 (clear the entry fields; the browser reload is intercepted), F6 (confirm/save), F7 (open `/register/print`), registered by a small script `wwwroot/batchShortcuts.js`. |
| F3 Product master CRUD | Blazor | Verified, one gap fixed | `Silo.Modules.Product` `AddProduct` is a **superset** of the legacy form: batch save (`SSaveProductBatch`), brand/group/class/sub-group, technical data, gallery image, Excel import, accounting-catalogue search (`SGetCusProductsForWeb` ≈ legacy `SGetCusProducts`), pack value = value × count-in-pack, size presets now come from the size master data (`Sizes[].Data`) instead of the hard-coded 75*75 / 60*60 / 60*120 values. Required fields are `DataAnnotations` (technical code, type, status, size, unit, group, brand) plus code/title from the server setting. **Gap fixed:** legacy required the pack value, the web did not, and a product saved with carton value 0 breaks registration (`SSavePrintBySerial` divides by it); `OnSubmitClick` now rejects a zero carton value or pack value (when those fields are enabled). Not guarded: the Excel import path. **Server note:** `SSaveProductBatch` builds its INSERT/UPDATE by string interpolation (injection risk) — not changed. |
| F4 Product type | Blazor | Verified, one gap fixed | `AddProductType` matches (`SSaveProductType` with parent id "1"; `ProductTypeParentsId` is "0" instead of legacy ""). Legacy required code and title, the web page did not; now enforced. Neither has a delete. |
| F5 Product picker | Blazor | Done | Reuses `ProductCodeModal`. |
| F6 Print + register tags | Blazor + Windows | Done for ZPL, ZMIN and FastReport (not run on hardware, not compiled) | `/register/print` (`PrintRegister`). **Mode** replaces the legacy `KindRegister`: *register* (1: per serial create/reuse the EPC `SCreateEpc`, `SRegisterTag`, print through the device host, `PrintUpdatePrintStatus`), *print only* (2 and 4: only print, then pair the tags on `/register/pair`; link on the page) and *print and register automatically* (3: print, then `SRegisterTag` with the serial's digits as EPC, letters A-Z removed like legacy `ReplaceAllEnglishAlphbet`, then mark the row). The mode is remembered per browser. Templates live on the workstation (`Formats\zpl`, `Formats\zmin`, `Formats\frx`). The hard-coded test EPC is not carried over. |
| F7 Print existing action (FastReport) | Blazor + Windows | Done (not compiled, not run) | Same page (`/register/print?actionId=`). **FastReport:** the legacy `Print_Formats\*.frx` templates are copied to `Silo.Windows.Register\Formats\frx` and run by the device host with the **same FastReport.NET build as the legacy app**, referenced from `Silo.Windows.Register\Packages\FastReport.dll` (plus `FastReport.Bars/Editor/VSDesign.dll`, copied next to the exe like the other vendor DLLs; no NuGet package). File naming is the legacy one `title_printer_brand.frx` (printer = 2nd token, "0" = host default; brand = 3rd token, used by the checkbox "only formats of the product's brand", the legacy `ControlPrintFormatByBrand`; names without a brand token are always shown). Parameters are set from the same token map as ZPL (`LabelFieldBuilder`): underscore names are also set with spaces (`سریال_محموله` and `سریال محموله`), plus the legacy extras (`تاریخ تولید شمسی/میلادی`, `مقدار هر کالا`, `واحد کالا`, `شیفت تولید`, `برند کالا`, `شانه کالا`, `عنوان کالا لاتین`, `RFID`), then the technical data and the product properties (properties win, as legacy). **Output:** "Printer" (one print job per serial, rendered as images) or **"One PDF"** (legacy `MergPrintToPDF`): all selected labels are appended to one prepared report and exported to a PDF that opens on the workstation (`%TEMP%\Silo.Windows.Register\labels-*.pdf`, deleted after a day); nothing is registered or marked for a PDF. **Preview first (PDF)** replaces the legacy preview window (the open-source engine has no previewer). The labels of a PDF travel in chunks of 25 in one job (`job` + `more`) because the socket message limit is 512 KiB. The legacy `PrintPreviewBeforePrint` confirmation dialog is not ported (use Preview). |
| F8 Register by barcode + RFID | Blazor + device | Done (not run on hardware) | `/register/pair` (`PairTag`): barcode typed by the scanner into the page (or sent by the host) plus EPC from the reader (single-read mode, restarted after each pair). Full legacy decision table (`SCheckTagEPCReturnProductSerial` → `SIsProductSerialExist` → `SRegisterTag`) with in-page confirmations; an error stops the run. Deviations: the edit path sends the real user id (legacy sent an empty username); "No" at the already-registered prompt is shown as a warning row and the run continues (legacy: silent success). **Auto print after register** (legacy `AutoPrintAfterRegister`, `PrintTestAfterRegister.frx`): checkbox on the page; after each successful pairing a FastReport receipt with the serial and `RFID` (the EPC) is printed through the device host (format and printer chosen on the page; a failure is a warning and does not stop the run). Not ported: single-serial mode opened from the print form, the hidden dev test button. |
| F9 Cancel registered tag | Blazor + device | Done (not run on hardware) | `/register/cancel` (`CancelTag`): reader single-read (mode 1, restarted after each lookup) or typed serial/EPC → `SPGetProductInfo` + `SGetProductBySerial` → in-page confirm → **`SCancelRegisterTagByDestinationAccess`**, which enforces the warehouse "allow cancel" flag on the server (legacy read it client-side from `SGetAllWarehouses`; this server's warehouse DTO does not carry it, and the old permission-leak defect is gone because every cancel is checked by the server). **Server fix:** that method had a SQL typo (`where WHERE`) in the tag-movement delete, which made every cancel fail; corrected in `WmsBusiness.cs`. The older `/product/revoke` page (Mode parameter) still exists and calls `SCancelRegisterTag`, which has **no** warehouse check; PairTag now links to the new page. Cancel is by EPC; the user id is sent as username (legacy sent the name). |
| F10 Change registration date | Blazor | Done | `/register/date`. Enforces the legacy rule: editable only when the inspect title is "بازرسی نشده" and `TagStatus` is 0 (production warehouses), checked on lookup and again before saving (the server does not enforce it). The lookup accepts a serial or an EPC (`SPGetProductInfo`); the reader is not wired to this page. |
| F11 Parent package | Blazor + device | Done (not run on hardware) | `/register/package` (`ParentPackage`): add serials (Enter or button; `SPGetProductInfo` accepts a serial), load by package code or EPC (`SPGetProductListByParentSerial`), save (`SRegisterNextlevelProductUnit`, parent EPC "-1" detaches a child), per-row remove. **Added:** "Read package tag" (reader mode 1, single read: clears the grid and loads the children already assigned to that tag, like legacy "1 - read tag") and **label printing through the device host** ("3 - print label", refused until the package is saved). Legacy printed a FastReport template `PrintPackFormat.frx` that is **not in the legacy repository**, so the label is a ZPL/ZMIN template of the host instead: sample `Formats/zpl/package-label.txt`. Tokens (legacy parameter names): `[کد_بسته_بندی]` package code, `[شناسه_تگ]` package EPC, `[تعداد_اقلام]` item count, `[جمع_مقدار_اقلام]` sum of values, `[لیست_سریال_ها]` lines `serial - title` (ZPL joins them with `\&` for `^FB`, ZMIN with a line break; at most 200 lines are sent). `GetLatestNextlevelProductUnitbyEpc` does not exist on the server, so the package code is re-read by EPC. Fixed legacy gotchas: a removed serial can be re-added without Clear. Not ported: barcode/typed radio (Enter works for both). |
| F12 Test register (single) | Blazor + device | Done | `/register/test`, connects to the device host through `LocalWebSocketComponent` (epc/barcode in, startReader/stopReader out). Not run against a real reader. |
| F13 Test register (group) | Blazor + device | Done (not run on hardware) | `/register/test/group` (`GroupTest`): continuous read (mode 2), new EPCs batched once a second into `SGetProductsByEpcs`, red/green rows, total and not-registered counters, Reset. The server query does not return an old-serial column (legacy showed an always-empty one), so that column is omitted. Lookups only run when new tags were read (legacy called the API every second). |
| F14 Registered-production report | Blazor | Verified; pivot added, time filter on the main report still open | `/report/register` is a redesigned dynamic-filter report (`SRepRegisterTagSummary` / `SRepRegisterTagDetails` with `ReportFilter`s), not a port. Covered filters: product code, line, from/to date, user, shift, QC, technical code, serial, size, document (ContractStatus), group, brand, register device, inspect status, dynamic fields; PDF/Excel export exist. **Missing vs legacy:** (1) from/to **time** filters (legacy business day starts 06:00; the web filter is date only); (2) the **tab-2 pivot** (line × reg-code × size with one column per quality status, `SRepRegisterTagSummaryGroupBySizeANDRegCode`) — no web page calls it; (3) the old-serial filter (legacy sent it wrongly anyway). The production line is optional in the web (required in legacy). **Added:** `/report/register/matrix` (`RegisterMatrix`, new server method **`SRepRegisterTagMatrix`**): the legacy tab 2 — line × technical code × size × product type with one column per quality, row totals and a total row, Excel export, filters product code, line, shift, quality, size, technical code (equals/contains), document, **Persian from/to date (default today → tomorrow = one business day) and from/to time**. The legacy server pivot `SRepRegisterTagSummaryGroupBySizeANDRegCode` is left untouched: it concatenates every filter into dynamic SQL (unquoted codes, `<> []`), so it only works for numeric codes and is injectable; the new method is fully parameterised, returns flat rows and the page pivots them (quality columns come from the data, not from all defined statuses). Dates are "yyyyMMdd" and the range is [from 06:00:01, to 06:00:01] using the first shift's start like the legacy server. Not ported: printing the pivot (FastReport `RepRegisterTab2.frx`); use Excel. The time filter is still missing on `/report/register` itself. |
| F15 Cancelled-registration report | Blazor | Verified, server fix | `/report/revoke` calls `SRepSCancelRegisterTag` with the same filters (serial, dates, user by id). **Fixed:** the server concatenated the filters into the SQL and left the serial **unquoted** (`ProductSerial= ABC1` is invalid SQL, so only numeric serials worked, and the values were injectable); the query is now parameterised. Note for data: the cancel stores the user **id** in `SoftDeleteUser` (`/register/cancel`, `SCancelRegisterTag` fallback) and this report joins `tbl_User` by id, but `TagHistory` (WmsBusiness line ~4038) looks the name up by **username**, so "cancelled by" can be empty there — not changed (depends on what old data holds). |
| F16 Data-quality report | Blazor | Verified | `/report/errorregister` runs the same two RPCs (`SRepRegisterTagByTagStatus0` with yesterday's Persian unix date, `SRepRegisterTagWithOutEPC`). No gap found. |
| F17 Settings | Windows | Done (subset) | `SettingsForm` + `%AppData%\Silo.Windows.Register\settings.json`. Lacks print-format, login, notification, shift and other print-related keys. |
| F18 Toast | Blazor | Covered | Uses the existing `Notification` service. |

### Windows device host (`Silo.Windows.Register`)

Done: settings store; `ReaderManager` (24-hex EPC normalization, T64 ignore list, de-duplication, single/continuous modes, device fault reporting); `SerialEpcReader`; `SimulatedEpcReader`; **vendor readers** `Rfid6061Driver` (RfidApiLib), `TinyHidDriver` (UhfReader_API_HID), `T64Driver` (VHLib) behind `VendorEpcReader`; `BarcodeWedgeBuffer` (splitter rule); `PrinterService` (raw ZPL); **label printing** (`LabelFormatStore`, `LabelTokens`, `ZplLabelPrinter`, `ZminLabelPrinter`, `PrintCommandHandler`); loopback WebSocket server (HTTPS origin allowlist, token, one client, 64 KiB limit); `MainForm` and `SettingsForm`. The host is built for **x64** (the native vendor DLLs are x64 only).

Message protocol: host to browser `{"type":"epc|barcode|pong|error","value":"..."}`, plus replies `{"type":"printers|formats","id":"1","ok":true,"value":"<json>"}` and `{"type":"printResult|printerStatus","id":"1","ok":true,"value":"message","code":0}`. Browser to host `{"type":"startReader","mode":1|2}`, `{"type":"stopReader"}`, `{"type":"ping"}`, `{"type":"print","id":"1","printer":"","data":"ZPL"}`, `{"type":"listPrinters","id":"1"}`, `{"type":"listFormats","id":"1"}`, `{"type":"printerStatus","id":"1","kind":"zpl|zmin","printer":""}`, `{"type":"printFormat","id":"1","kind":"zpl|zmin","format":"name","printer":"","writeRfid":false,"fields":{"token":"value"},"technicalData":{"name":"value"}}`. FastReport: `{"type":"printFormat","kind":"frx","format":"name","printer":"","output":"print|pdf","items":[{"fields":{},"technicalData":{}}],"job":"id","more":true}` (`more` = only collect; the call without it prints/exports the whole job; the host replies `printResult` with the PDF path). Templates use `[token]` placeholders (single-pass substitution; values for ZPL have `^` and `~` removed). Token names are the legacy Persian ones (`سریال_محموله`, `شناسه_تگ`, ...), built in `LabelFieldBuilder`.

### Remaining work

1. **Vendor reader drivers:** implemented from the legacy calls but **not tested on real readers** (and the code has not been compiled in CI). T64 reads only the first tag per poll, as legacy did. Tiny ignores the configured power, as legacy did.
2. **F6/F7 printing:** ZPL, ZMIN, FastReport (`.frx` to a printer or one PDF) and the three register modes are implemented. FastReport uses the legacy calls (`PrintSettings.Printer`, `Print()`, `PDFExport`, `Prepare(true)` to append labels of one job), all in `Printing/FastReportPrinter.cs`. **To verify on the workstation:** the .NET Framework FastReport 2017 DLL running inside the net9 host (loading, Persian text, printing); "Microsoft Print to PDF" in a file name opens a save dialog like legacy. The ZMIN flow needs a real ZMIN printer. The F11 package label is ZPL/ZMIN (its FastReport template is missing from the legacy repository). Not ported: the confirmation dialog before printing, `PrinterModelForAutoPrintAfterRegister` as a separate setting (the page chooses the printer), the legacy `de\` and `reports\` templates.
3. **F8:** done (`/register/pair`). The host's keyboard-wedge buffer still only sees keys while the host window has focus; the web pages therefore take the barcode in their own focused input.
4. **F9:** done (`/register/cancel`). Open: decide whether to retire or restrict `/product/revoke` (no warehouse check) and add the claim for the new route.
5. **Verification of "existing" items:** done for F3, F4, F14, F15, F16 (see the status table). Open from it: F14 time filters on the main `/report/register` and printing of the pivot (the pivot itself is done); `SSaveProductBatch` SQL concatenation; `TagHistory` cancelled-by lookup; the Excel product import does not check carton/pack values.
6. **Hardware testing** of the device host, readers, scanner, printer, and browser behaviour for `ws://127.0.0.1` from an HTTPS page (mixed content / private network access).
7. **Host setup:** allowed origin and token must be configured in the Windows app. The token is typed once in the component and then reused (auto-connect with retry, `DeviceRpc` request/reply now live in `Silo.Shared.Components`).
8. **Localization:** step 1 done with the **existing** `TextResources` keys only (no new key): common words and messages on the new pages (clear, start, stop = `Pause`, yes/no, documents, average, remaining, EPC = `TagEPC`, "tag is not registered", "wait to read tag/barcode", "no serial/document/product found", "not allowed", Excel/format/size errors, select all, refresh, revoke, separator, from/to serial, export Excel, ...) and in the Windows host (`Settings`, `Save`, server address, device type, digits count, separator). The device host loads the texts like the web app (`FormalDataCache.GetTextResources`): `Localizer.LoadAsync` posts to the anonymous `TextResource/ReadAll` of the API (`http://{ServerIP}/RfidCore/v2/`, the `ServerIP` of the host settings) when the main window is shown and again after the settings are saved, keeps them in `Silo.Shared.ResourceManager` and the windows re-apply their texts; if the server is unreachable the log says so and the English fallbacks stay. Texts changed in the host forms are applied in the form constructors / `ApplyTexts` (not in the Designer files). **Still English (no matching key yet):** sentences, status lines and composite labels (printing/pairing/batch messages, FastReport options, the host's server/reader status lines and most settings labels) — they need new keys in step 2.
9. **UI/UX of the web pages (done, not compiled):** the nine Register pages were rebuilt following `.github/ui-design-system.md` (cards `border-0 shadow-sm`, Bootstrap utilities, Material icons, `TelerikLoaderContainer` as the last element, no inline styles). Reusable parts live in `Silo.Modules.Register/Components`: `PageHeader` (title, purpose, device status, links, collapsible "how to use this page" steps), `SectionCard` (numbered step with hint), `FormField` (label, hint, error), `ScanInput`, `OptionSelect`, `PrinterSelect`, `LabelFormatSelect`, `RegisterGrid` (TelerikGrid with paging, empty message, row colours, fresh data snapshot), `StatusMessage`, `ConfirmBar` (inline yes/no instead of dialogs), `DeviceStatusBadge`, `PageLoader`. HTML tables, selects, checkboxes and inputs are Telerik components (`TelerikGrid`, `TelerikDropDownList`, `TelerikCheckBox`, `TelerikTextBox`, `TelerikNumericTextBox`, `TelerikButton`); the device bar (`LocalWebSocketComponent`) is Persian too. All visible texts are Persian: `TextResources` where a key exists, otherwise the hardcoded Persian in `Silo.Modules.Register/RegisterTexts.cs` (one place, ready to be moved to real keys). Row colours come from `wwwroot/register.css` (Bootstrap subtle palette). Behaviour change to know: Telerik text boxes commit on Enter/blur (not on every key), so buttons are no longer disabled while a field is empty; the handlers answer with a clear message instead. Still native: the `.xlsx` `InputFile` and the browser `confirm` of single-product save.
10. **Claims/permissions** for `/register/batch`, `/register/date`, `/register/package`, `/register/test`, `/register/print`, `/register/pair`, `/register/test/group`, `/register/cancel`, `/report/register/matrix`.
11. **Unmigrated F17 settings** and the legacy migration of old settings values.
12. **F2 gaps:** done since: operation navigation, the pack-value-from-total-weight helper, the user whitelist (`Register:BatchAllowedUsers`; the legacy list was `admin, f.entezari, m.shabani, s.dehghani, f.kafinia, mr.abedini, j.saeidi, m.heidari, m.mofatah, fathi, a.dost, m.Ghafeli`) and the F5/F6/F7 shortcuts. **Still open:** the `.xls` Excel format (only `.xlsx`), the dead "get from Api" tab (no logic in legacy either) and the `RegisterCountEvryTimeIsOne` / `AllowShowOnlyPrintFormBeforeSavePrintAction` settings (the print link appears after saving).

---

## 1. Application overview

- **Product:** a Windows desktop client for a factory/warehouse "register and label" process. Operators define a production batch (product, production line, shift, warehouse, roll/document code, count), the system creates one **print row per serial** on the server, labels (barcode and/or RFID tag) are printed, and each serial is **registered** — i.e. bound to an RFID tag EPC — so the item can be tracked afterwards (inspection, warehouses, packages/parents, reports). Supporting screens: master data (products, product types), cancel a registration, change the registration date, group serials into a parent package, QA tests of label-vs-tag correspondence, and reports.
- **Company/branding:** "آویژه" (Avizhe); window titles/labels Persian. The shipped sample catalogue/templates are for network/security equipment (camera, NVR, switch) in one deployment and ceramic tiles (size strings `75*75`, `60*60`, `60*120`; statuses `SPC`, `اختلاطی`, `دپو`) in another — the product model is generic.
- **Runtime:** .NET Framework 4.8 WinForms, single project `RfidConnectRegister.csproj`, namespace `RfidConnectRegister`. Windows-only native hardware drivers (RFID readers, label printer SDK). Async/await is used throughout with many `async void` UI handlers and WinForms timers.
- **Architecture:** thin client. All business data is read/written through one HTTP RPC endpoint (§4). There is **no local database use** in the flows read (a generic SQL helper exists but is not wired), **no automated tests**, **no authentication token** (the user id/name is passed as a parameter).
- **Project layout**
```
RfidConnectRegister/
  Program.cs                 entry: load ApplicationConfig.log → RestApi.Ip → Application.Run(FrmLogin)
  UI/                        FrmLogin, formSetting (FormSetting), frmShowAlert, Form1 (vendor demo, not part of the app)
  UI/StoreProduct/           the business forms (hub, print, register, cancel, change-date, parent, tests, reports, master data)
  BussinessLogic/
    RfidConnectApi/          ApiBusiness (all RPC calls), WorkingWithApiConnector/Connector (HTTP), RestAPI (legacy, unused)
    Classes/                 BLLConnectToUHFReader (desk RFID readers), GetSettingDictionary, ConfigKeys, AvizheExcel, Tools, GetComPort, ShamsiCalendar…
    ZPLHelper(.Base)         raw ZPL printing
    Time, Tools              date helpers, JSON/DataTable helpers
    SchasualedSmsCreator     30-s notification poller;  TaskTrayApplicationContext (tray);  sms.cs (dead);  AppRegister (licence, disabled)
  DML/                       DTOs
  DataAccess/                Items (session globals), DataAccess (generic ADO.NET, unused)
  Properties/                Settings.settings (user-scope settings)
  bin/Debug/                 runtime assets: Print_Formats\*.frx, reports\*.frx, zpl_Formats\*.txt, zmin_Formats\*.lsf, vendor DLLs
```

---

## 2. Screen map, navigation and operating modes

```
Program → FrmLogin (F1) ──(gear)──► FormSetting (F17, modal)
   │ success
   ├─ NotifApp = true  → hide login, tray icon + 30-s notification poller (S5) → menu: "گزارش نوتیف ها" → frmRepRegisterTag (F14)
   └─ NotifApp = false → frmStoreRegisterTags = HUB (F2)         (closing the hub exits the application)
        HUB buttons / hotkeys
        ├─ product picker ........ frmSearchProducts (F5, modal)
        ├─ "ثبت (F6)" save the batch (creates print rows on the server)
        ├─ "چاپ لیبل >> (F7)" ... KindRegister=1 → frmPrintRegsiterTags (F6)      ZPL / ZMIN / FastReport + register at print
        │                         KindRegister=2|3|4 → frmOnlyPrint (F7)           FastReport only (+ register-by-barcode dialog F8)
        │                              └─ "رجیستر تگ ها" → frmRFIDRegisterByBarfid (F8, modal; kinds 2 and 4)
        │                                     ├─ frmCancelRegisterTag (F9)   └─ frmTestRegister (F12)
        ├─ "ابطال رجیستر تگ" ..... frmCancelRegisterTag (F9, modal)
        ├─ "تست رجیستر تگ" ....... KindRegister=2 → frmTestRegister (F12) else frmTestRegisterGroup (F13)  (modal)
        ├─ "گروه بندی" ............ frmSetParent (F11)     (button hidden unless SetParentForSerials)
        ├─ "گزارش رجیستر تگ ها" .. frmRepRegisterTag (F14)
        ├─ "تنظیمات" ............. FormSetting (F17, modal)
        └─ hidden by default in the designer: frmChangeTagRegisterData (F10), frmRepCancelRegister (F15), frmRepErrorInRegisterTag (F16), frmDefineProducts (F3), frmDefineProductType (F4)
Also: frmShowAlert (F18) is only created by the notification poller.   frmPrintRegsiterTags also has shortcut buttons to F9 and F13.
```

**`KindRegister` (setting; UI combo index = stored value)** selects the whole print/register flow:

| Value | Combo text (verbatim) | Flow |
|---|---|---|
| 0 | نا مشخص (undefined) | no form is opened by the print button (undefined) |
| 1 | رجیستر از طریق RFID Lable Printer | **F6**: the label printer encodes the RFID tag while printing; tag registered during print (EPC source ⚠ see F6) |
| 2 | رجیستر از طریق بارفید | **F7 + F8**: labels printed with FastReport (barcode only), then the operator scans barcode + RFID tag with a desk reader to pair them (F8). Test button → F12 |
| 3 | رجیستر با چاپ بارکد | **F7**: registration happens at print time; the EPC is the serial's digits (no physical tag is read). "RFID registration" button hidden |
| 4 | رجیستر تک سریال با ریدر رو میزی | **F7 + F8** in single-serial mode (`ActiveRegisterFormByOneSerial`) |

The default stored value is `"1"`.

**Persistent state while running:** `DataAccess.Items.OnlineUserCode / OnlineUserName / OnlineUserRealName` (set at login), `Properties.Settings.Default.*`, `GetSettingDictionary.DicSetting`. Nothing else is persisted client-side.

---

## 3. Configuration

### 3.1 Stores
1. **Typed user settings** — `Properties.Settings.Default.*` (`Settings.settings`, user scope, written to the user's config on `Save()`).
2. **File-backed dictionary** — `ApplicationConfig.log` next to the exe: **one line** `key=value/key=value/…` read at startup into `GetSettingDictionary.DicSetting`; rewritten by `SaveSettingInFile()`. Keys defined in `ConfigKeys` (below). If the file is missing/unreadable the app shows "در ثبت فایل تنظیمات مشکلی بوجود آمده است" and exits.
The two stores are used by different code and **not synchronized** (see S2). The login form's server-IP editor writes only the dictionary; `ApiBusiness` reads `Settings.Default.ServerIP` — **⚠ Defect**: an IP saved from the login screen does not change the URL used by `ApiBusiness` (the settings screen F17 writes the typed store).

### 3.2 `ConfigKeys` (dictionary keys)
`ServerIP, HaveLogin, BarcodeLength, ShowSaveDialog, PrinterInOperation, BarcodeSpliter, BarcodeIndex, CompanyName, ReportFileName, User, PrinterModel, DeviceIP, DeviceID, PrintPaperType, BarcodeType, TableModel, BarcodeLength1, BarcodeLength2, COMPairDevice, ReaderPower, ReaderPort, MergPrintToPDF, KindRegister, ControlPrintFormatByBrand, GetListSerial, AutoDetecteShift, SerialLength, NotifApp`.
Dictionary reads found in forms: `ReaderPort` (F9–F12 → `KeyNotFoundException` if missing), `BarcodeSpliter` (F8), `ServerIP` (F1).

### 3.3 Settings table (declared in `Settings.settings`; "used by" = code actually reading it)

| Setting | Type | Default | Used by (code read) |
|---|---|---|---|
| `ServerIP` | String |  | ApiBusiness, FrmLogin, Program, formSetting |
| `DeviceID` | String | `2` | — (declared only) |
| `DeviceIP` | String |  | — (declared only) |
| `TableModel` | String | `P1` | formSetting (stores the combo index) |
| `CountReader` | Int32 |  | — (declared only) |
| `ReaderPower` | Int32 | `7` | BLLConnectToUHFReader, formSetting |
| `ConnectionString` | String |  | — (declared only) |
| `PortNumber` | String | `COM9` | GetComPort, formSetting |
| `COMPairDevice` | String | `Device1=COM4/` | — (declared only) |
| `BarcodeLength` | Int32 | `0` | — (declared only) |
| `SoftwareVersion` | String | `1.0.0` | — (declared only) |
| `PrinterModel` | String |  | formSetting, frmOnlyPrint, frmPrintRegsiterTags, frmRepRegisterTag, frmSetParent |
| `ShowSaveDialog` | Boolean | `True` | formSetting |
| `PrinterInOperation` | Boolean | `False` | formSetting |
| `BarcodeSpliter` | String |  | formSetting, frmRFIDRegisterByBarfid |
| `BarcodeIndex` | Int32 | `0` | formSetting |
| `CompanyName` | String | `آویژه` | formSetting |
| `KindPrint` | String | `A5` | — (declared only) |
| `RfidConnectServerIp` | String | `localhost:97` | — (declared only) |
| `PrintFileAdress` | String | `reports\\roka_Tag.frx` | — (declared only) |
| `SmsUserName`, `SmsPassword`, `SmsNumber1`, `PanelType`, `api_key` | String | (blank; values must not be copied) | sms (dead code) |
| `KindRegister` | String | `1` | formSetting, frmOnlyPrint, frmStoreRegisterTags |
| `MergPrintToPDF` | String | `1` | formSetting, frmOnlyPrint, frmPrintRegsiterTags |
| `DeskReaderType` | Int32 | `1` | BLLConnectToUHFReader, formSetting |
| `DeskPower` | Byte | `0` | BLLConnectToUHFReader (T64) |
| `DeskUHFReader_Port` | String |  | BLLConnectToUHFReader, formSetting, frmTestRegisterGroup |
| `SerialLength` | Int32 | `5` | formSetting, frmRFIDRegisterByBarfid, frmSetParent |
| `ControlPrintFormatByBrand` | String |  | formSetting, frmOnlyPrint |
| `AutoDetecteShift` | String |  | formSetting, frmStoreRegisterTags |
| `GetListSerial` | String |  | formSetting, frmStoreRegisterTags |
| `BarcodeLength1`, `BarcodeLength2` | String |  | formSetting only |
| `ReportFileName` | String |  | formSetting only |
| `HaveLogin` | Boolean | `False` | formSetting, frmStoreRegisterTags |
| `NotifApp` | Boolean | `False` | FrmLogin, formSetting |
| `PrintPaperType` | String |  | formSetting only (window title) |
| `BarcodeType` | String |  | formSetting only |
| `IsZpl` | Boolean | `False` | formSetting, frmPrintRegsiterTags |
| `MultiProductInOnePrintActionId` | Boolean | `False` | frmStoreRegisterTags — **no UI in F17** (config-file/user-settings edit only) |
| `CheckRegisterByDocItems` | Boolean | `False` | frmStoreRegisterTags — **no UI in F17** |
| `CountInPackEqualCount` | Boolean | `False` | frmStoreRegisterTags — **no UI in F17** |
| `ComputeAverageProductPackValueFromAllWeight` | Boolean | `False` | frmStoreRegisterTags — **no UI in F17** |
| `AllowShowOnlyPrintFormBeforeSavePrintAction` | Boolean | `False` | formSetting, frmStoreRegisterTags |
| `RegisterCountEvryTimeIsOne` | Boolean | `False` | formSetting, frmStoreRegisterTags |
| `PrintPreviewBeforePrint` | Boolean | `False` | formSetting, frmOnlyPrint |
| `ActiveRegisterFormByOneSerial` | Boolean | `False` | formSetting, frmOnlyPrint |
| `PrinterModelForAutoPrintAfterRegister` | String | `Canon MF3010` | frmOnlyPrint — **no UI in F17** |
| `AutoPrintAfterRegister` | Boolean | `False` | frmOnlyPrint — **no UI in F17** |
| `IgnoreEpcList` | String | two sample EPCs, comma-separated | BLLConnectToUHFReader (T64), formSetting |
| `SetParentForSerials` | Boolean | `False` | formSetting, frmStoreRegisterTags |

**Feature-flag meanings** (all verified in the forms in §7): `MultiProductInOnePrintActionId` – several products in one action (hub multi-grid); `CheckRegisterByDocItems` – a count may not exceed the document's remaining quantity; `CountInPackEqualCount` – the checked quantity = count × count-in-pack; `ComputeAverageProductPackValueFromAllWeight` – the "average weight" helper replaces the serial-list button; `HaveLogin` – only whitelisted users may save a batch; `GetListSerial` ("1" shows the serial-list button); `AutoDetecteShift` ("1" auto-selects the shift by the clock); `RegisterCountEvryTimeIsOne` – the count box is fixed to 1; `AllowShowOnlyPrintFormBeforeSavePrintAction` – the print form can be opened before saving; `ActiveRegisterFormByOneSerial` – F8 is opened for one specific serial; `AutoPrintAfterRegister` – after a successful F8 pairing a second FastReport receipt is printed; `ControlPrintFormatByBrand` – only label templates for the product's brand are offered; `MergPrintToPDF` – FastReport output is merged into one PDF instead of printing; `PrintPreviewBeforePrint` – the first row is previewed before printing; `IsZpl` – F6 uses ZPL (else ZMIN); `SetParentForSerials` – show the grouping button; `NotifApp` – tray + polling mode.

---

## 4. Server API contract

### 4.1 Transport (from `ApiBusiness`, `WorkingWithApiConnector`, `Connector`)
- **URL:** `http://{Settings.ServerIP}/RfidCore/v1/Wms/PostObject` (e.g. `localhost:97`). The URL is built **once per `ApiBusiness` instance**. HTTP only, no TLS option.
- **Request:** `POST`, `Content-Type: application/json`, UTF-8, timeout **60 s**:
```json
{ "interface": "RestAPI", "method": "<RpcName>", "parameters": { "<name>": <value>, ... } }
```
- **Response:** `{ "successful": true|false, "value": <payload> }`. Clients test `successful` and then read `value` (a boolean, a string, a JSON array of objects, or — for the pivot report — a JSON *string* that is parsed a second time).
- **Authentication:** none beyond sending the user id/name as parameters (see "user argument" column). No session, no token, no retry.
- **Failure behaviour:** any transport exception makes the low-level call return `null`, after which `ApiBusiness` throws `NullReferenceException`; list methods return `null` when `successful` is false. A catch-all helper (`LoadUnDirectAsync`) can also produce `{"Successful": true, "Value": false}` (capital letters, which the callers' lower-case lookups will not find). A rewrite must define its own error semantics; legacy callers mostly swallow errors and show a generic message.
- **Legacy client:** `RestAPI.cs` (`http://{Ip}/RfidConnectApi/RfidConnectApiHandler.ashx`, same JSON envelope) is an older endpoint with many unrelated methods (pipe/coupling QC: `Receipt`, `PrintDelete`, `Login`, `IsTagEpcExist`, …). **It is not used by `ApiBusiness` or any form read here** — only the static `RestApi.Ip` assignment at startup remains.

### 4.2 RPC catalogue (generated from `ApiBusiness.cs`; parameter names are the exact JSON keys)

User-id argument convention (verified): parameters named `userToken` / `PrintUser` / `ProductRegUser` carry `Items.OnlineUserCode`; parameters named `username` (register/cancel) carry `Items.OnlineUserName`; `GetAllUser` sends the code; `SGetNotif_WinFormService` sends `OnlineUserRealName` as `User`. Many list RPCs send `userToken=""`.

| RPC `method` | Client method | Parameters (JSON names) | Returns |
|---|---|---|---|
| `TSLogin` | `Login` | `username`, `password` | `bool` |
| `SGetDocumentsByDocType` | `GetAggDocsByDocTypeAndStatus` | `documentType` | `List<DML.GetAllAggDocDto>` |
| `SGetDocumentData` | `GetDocumentData` | `documentKey`, `documentType` | `JToken` |
| `TSLoginReturnName` | `TSLoginReturnName` | `UserId` | `string` |
| `SGetDynamicFieldsByActionTypeId` | `GetDynamicFieldsByActionTypeId` | `actionTypeId` | `List<DynamicFieldDto>` |
| `SPSearchProductWeb` | `GetAllProductByTechnicalData` | `search` | `string` |
| `SGetAllProductsDefined` | `GetAllProduct` | `userToken` | `List<Product>` |
| `SGetDynamicFieldsBySourceAndDestination` | `GetDynamicField` | `sourceWarehouseCode`, `destinationWarehouseCode` | `List<DynamicFieldDto>` |
| `SChangeRegisterDate` | `ChangeRegisterDate` | `ProductSerial`, `NewDate`, `NewTime` | `bool` |
| `SCheckProductSerial` | `CheckProductSerial` | `ProductSerial` | `bool` |
| `SCheckTagEPCReturnProductSerial` | `CheckTagEPC` | `TagEpc` | `string` |
| `SIsProductSerialExist` | `IsProductSerialExist` | `ProductSerial`, `username`, `deviceIp`, `deviceId` | `int` |
| `SGetAllWarehouses` | `GetAllWarehouses` | — | `List<WarehouseDto>` |
| `SGetCusProducts` | `GetAllWMSProduct` | — | `List<Product>` |
| `SGetProductsByEpcs` | `GetTagsInfo` | `epcList`, `userToken` | `List<Tag>` |
| `SPGetProductInfo` | `GetTagInfo` | `tagEpc` | `List<Tag>` |
| `SPGetProductListByParentSerial` | `GetTagListByParentCode` | `parentSerial`, `parentEpc` | `List<Tag>` |
| `GetPrintData` | `GetPrintInfo` | `serials`, `userToken` | `List<Tag>` |
| `SRepRegisterTagDetailsForWinApp` | `RepRegisterTag` | `filter` | `List<Tag>` |
| `SRepSCancelRegisterTag` | `RepSCancelRegisterTag` | `ProductSerial`, `FromDate`, `ToDate`, `User` | `DataTable` |
| `SGetRegisteredPrints` | `RepSGetNotRegisteredPrints` | — | `DataTable` |
| `SRepRegisterTagByTagStatus0` | `RepRegisterTagByTagStatus0` | `FromRegisterShamsiUnixDate` | `DataTable` |
| `SGetRemainRegisterByDoc` | `SGetListOfRemainProductByDocumentId` | `DocumentId` | `DataTable` |
| `SRepRegisterTagWithOutEPC` | `SRepRegisterTagWithOutEPC` | — | `DataTable` |
| `SRepRegisterTagSummaryForRegisterWinApp` | `RepRegisterTagTajamoeeProductCode` | `filter` | `List<Tag>` |
| `SGetNotif_WinFormService` | `SGetNotif_WinFormService` | `DateTimeLastCheck`, `User` | `string` |
| `SRepRegisterTagSummaryGroupBySizeANDRegCode` | `RepRegisterTagSummaryGroupBySizeANDRegCode` | `filter` | `DataTable` |
| `SCreateEpc` | `CreateEpc` | `serial`, `productCode`, `refCode`, `epc`, `type` | `string` |
| `SSavePrintBySerial` | `SavePrintBySerial` | `ProductSerial`, `ProductCode`, `ProductCount`, `PrintActionId`, `ProductProductionLine`, `ProductProductionShift`, `PrintCount`, `ProductContractType`, `ProductOldSerial`, `Location`, `DocumentId`, `PrintUser`, `WareHouseCode`, `ProductProperties` | `bool` |
| `SGetNewPrintActionId` | `GetNewPrintActionId` | `userToken` | `string` |
| `GetLatestNextlevelProductUnitbyEpc` | `GetLatestNextlevelProductUnitbyEpc` | `EpcCode` | `string` |
| `SaveProduct` | `SaveProduct` | `ProductCode`, `ProductTitle`, `ProductENTitle`, `ProductPackValue`, `ProductPackWeight`, `ProductPackVolume`, `ProductCountInPack`, `ProductValue`, `ProductTechnicalCode`, `ProductProperties`, `ProductType`, `ProductStatus`, `ProductSize`, `ProductUnit`, `ProductRegUser` | `bool` |
| `SRegisterNextlevelProductUnit` | `SRegisterNextlevelProductUnit` | `productUnitEpc`, `listEpc`, `userToken` | `bool` |
| `SDeletePrintByPrintId` | `SDeletePrintByPrintId` | `PrintId` | `bool` |
| `SDeleteProduct` | `DeleteProduct` | `ProductCode` | `bool` |
| `SSaveProductType` | `SaveProductType` | `ProductTypeTitle`, `ProductTypeParentId`, `ProductTypeParentsId`, `ProductTypeCode`, `userToken` | `bool` |
| `SRegisterTag` | `SaveRegisterTag` | `productSerial`, `productCode`, `productCount`, `epcList`, `username`, `deviceIp`, `deviceId`, `RegisterProperties` | `bool` |
| `SGetAllProductStatus_winapp` | `GetAllProductStatus` | `userToken`, `haveNotSelect` | `List<ProductStatus>` |
| `SGetAllProductType` | `GetAllProductType` | `userToken`, `haveNotSelect` | `List<ProductType>` |
| `SGetAllProductPropertyC` | `GetAllProductPropertyC` | `userToken`, `haveNotSelect` | `List<ProductPropertyC>` |
| `SGetPrintDataByProductSerial` | `GetPrintDataByByProductSerial` | `ProductSerial`, `userToken` | `List<Print>` |
| `SGetPrintListByPrintActionId` | `GetPrintListByPrintActionId` | `PrintActionId`, `userToken` | `List<Print>` |
| `PrintUpdatePrintStatus` | `PrintUpdatePrintStatus` | `productSerial` | `bool` |
| `SCancelRegisterTag` | `CancelRegisterTag` | `TagEpc`, `username`, `deviceId`, `deviceIp` | `bool` |
| `GetAllProductPropertyAData` | `GetAllProductionLine` | `userToken` | `List<ProductionLine>` |
| `GetAllProductPropertyBData` | `GetAllProductionShift` | `userToken` | `List<ProductionShift>` |
| `GetAllUser` | `GetAllUser` | `userToken` | `List<User>` |

**Fixed/derived parameter values** (so the new client sends the same payloads):
- `SGetDynamicFieldsByActionTypeId.actionTypeId`: an **int** (always `0` in the screens read).
- `SPSearchProductWeb.search`: `{MProductTitle:"-1", MProductCode:<code>, MTechCode:"-1", MSize:"-1", MQuality:"-1"}`.
- `SIsProductSerialExist`: `username`, `deviceIp`, `deviceId` always `""`.
- `SRegisterTag`: `productCount=""` (as called by the screens), `epcList` = `[EPC]` (string array), `deviceIp="0"`, `deviceId="0"`, `RegisterProperties=""`.
- `SCancelRegisterTag`: `deviceId="0"`, `deviceIp="0"`.
- `SCreateEpc`: `epc=""`, `type="1"` (call exists but no screen uses it).
- `SSavePrintBySerial`: see F2 — `ProductCount` string (`"0"` if null), `ProductProductionLine/Shift` `"0"` if null, `Location="0"`, `ProductProperties` = a JSON **object** (not a string), result `value` is a string: **empty = failure**, non-empty = success.
- `SaveProduct`: sends `ProductPackValue`, `ProductPackWeight`, `ProductPackVolume`, `ProductCountInPack` as `"0"` when empty, `ProductProperties`/`ProductType` as `""` when null; all values are strings.
- `SGetAllProductStatus_winapp`, `SGetAllProductType`, `SGetAllProductPropertyC`: `haveNotSelect=false`.
- `SGetNotif_WinFormService.DateTimeLastCheck`: `DateTime.ToString()` in the **current culture** (⚠ culture-dependent string).
- Report filter object (`filter`): see §5.4.

### 4.3 Server JSON → DTO mapping (what the client reads; unspecified fields are not read)

**SGetDynamicFieldsByActionTypeId** → title, id, relatedTitle1
  (renamed: Title←title, Id←id, RelatedTitle1←relatedTitle1)
**SGetAllProductsDefined** → ProductCode, ProductCountInPack, ProductENTitle, ProductPackValue, ProductPackVolume, ProductPackWeight, ProductProperties, ProductSize, ProductStatus, ProductTechnicalCode, ProductTitle, ProductType, ProductUnit, ProductValue, ProductBrand, ProductGroup, ProductTechnicalData, ProductBrandTitle, ProductGroupTitle, ProductStatusTitle, ProductTypeTitle, ProductSizeTitle
**SGetDynamicFieldsBySourceAndDestination** → ActionType, FieldType, Title
**SGetAllWarehouses** → epc, destinationCode, destinationTitle, operationalType, inventoryType, isDefault, isActive, id, Epc
  (renamed: dict←epc, DestinationCode←destinationCode, DestinationTitle←destinationTitle, OperationalType←operationalType, InventoryType←inventoryType, IsDefault←isDefault, IsActive←isActive, Id←id)
**SGetCusProducts** → mat_Code, mat_Name
  (renamed: ProductCode←mat_Code, ProductTitle←mat_Name)
**SGetProductsByEpcs** → TagEpc, ProductCode, ProductSerial, ProductCount, ProductName, DeviceIp, ContractStatus, RegCode
  (renamed: ProductOldSerial←DeviceIp)
**SPGetProductInfo** → TagEpc, ProductCode, ProductSerial, ProductCount, ProductName, ProductTypeTitle, ProductStatusTitle, TagStatus, ProductProperties, ProductSize, RegCode, fld_ProductPropertyATitle, fld_ProductPropertyBTitle, ProductPackWeight, ProductPackVolume, ProductCountInPack, ProductValue, ProductUnit, fld_ProductPropertyAId, fld_ProductPropertyBId, TagZone, TagStatusTitle, Username, TagRegisterShamsiUnixDate, FreezeStatusTitle, InspectStatusTitle, TagInDestinationId, TagTreeParentsEpc, TagTreeParentSerial
  (renamed: TagRegisterUser←Username)
**SPGetProductListByParentSerial** → TagEpc, ProductCode, ProductSerial, ProductCount, ProductName, ProductTypeTitle, ProductStatusTitle, TagStatus, ProductProperties, ProductSize, RegCode, fld_ProductPropertyATitle, fld_ProductPropertyBTitle, ProductPackWeight, ProductPackVolume, ProductCountInPack, ProductValue, ProductUnit, fld_ProductPropertyAId, fld_ProductPropertyBId, TagZone, TagStatusTitle, Username, TagRegisterShamsiUnixDate, FreezeStatusTitle, InspectStatusTitle, TagInDestinationId, TagTreeParentsEpc, TagTreeParentSerial
  (renamed: TagRegisterUser←Username)
**GetPrintData** → ProductCode, ProductSerial, ProductCount, ProductName, ProductStatus
  (renamed: ProductStatusTitle←ProductStatus)
**SRepRegisterTagDetailsForWinApp** → ProductCode, ProductSerial, Count, ProductName, ProductStatusTitle, RegCode, DateTime, Username, Line, Shift, Size, DateTimeEnter, Status
  (renamed: ProductCount←Count, TagRegisterShamsiUnixDate←DateTime, TagRegisterUser←Username, fld_ProductPropertyATitle←Line, fld_ProductPropertyBTitle←Shift, TagEpc←Size, ProductProperties←DateTimeEnter, TagStatusTitle←Status)
**SRepSCancelRegisterTag** → ProductSerial, ProductCode, ProductName, ProductRegCode, ProductCount, SoftDeleteUser, SoftDeleteDate, RegisterUser, RegisterDate
**SGetRegisteredPrints** → ProductSerial, ProductCode, ProductName, ProductCount, TagEpc, RegisterFlag
**SRepRegisterTagByTagStatus0** → ProductSerial, ProductCode, ProductName, RegCode, ProductCount, Username, TagRegisterShamsiUnixDate
**SGetRemainRegisterByDoc** → Count, ProductCode
**SRepRegisterTagWithOutEPC** → ProductSerial, ProductCode, ProductName, RegCode, ProductCount, Username, TagRegisterShamsiUnixDate
**SRepRegisterTagSummaryForRegisterWinApp** → ProductCode, ProductName, RegCode, ProductStatusTitle, Count, SumCount, Size, NotEnterCount, LineTitle
  (renamed: ProductSerial←Count, ProductCount←SumCount, ProductSize←Size, fld_ProductPropertyATitle←LineTitle)
**SGetAllProductStatus_winapp** → Code, Title
  (renamed: ProductStatusCode←Code, ProductStatusTitle←Title)
**SGetAllProductType** → Code, Title
  (renamed: ProductTypeCode←Code, ProductTypeTitle←Title)
**SGetAllProductPropertyC** → Code, Title
  (renamed: ProductPropertyC_Id←Code, ProductPropertyC_Title←Title)
**SGetPrintDataByProductSerial** → Id, ProductSerial, ProductCode, ProductName, ProductENTitle, ProductDescription, ProductTypeTitle, ProductCount, ProductItemCount, ProductCountInPack, ProductUnit, ProductSize, ProductRegCode, ProductStatusTitle, ProductWeight, ProductVolume, ProjectCode, fld_ProductPropertyATitle, fld_ProductPropertyBTitle, PrintType, PrintUser, PrintFlag, RegisterFlag, InputFileName, PrintActionDateTime, ProductContractType, ProductOldSerial, DocumentId, ProductTechnicalData, TagEpc, ProductGroup, ProductBrand, ProductBrandTitle, ProductGroupTitle, ProductSizeTitle, ProductProperties, ProductProductionLine, ProductProductionShift
  (renamed: ProductTitle←ProductName, ProductProductionLineTitle←fld_ProductPropertyATitle, ProductProductionShiftTitle←fld_ProductPropertyBTitle)
**SGetPrintListByPrintActionId** → Id, ProductSerial, ProductCode, ProductName, ProductENTitle, ProductDescription, ProductTypeTitle, ProductCount, ProductItemCount, ProductCountInPack, ProductUnit, ProductSize, ProductRegCode, ProductStatusTitle, ProductWeight, ProductVolume, ProjectCode, fld_ProductPropertyATitle, fld_ProductPropertyBTitle, PrintType, PrintUser, PrintFlag, RegisterFlag, InputFileName, PrintActionDateTime, ProductContractType, ProductOldSerial, DocumentId, ProductTechnicalData, TagEpc, ProductGroup, ProductBrand, ProductBrandTitle, ProductGroupTitle, ProductSizeTitle, ProductProperties, ProductProductionLine, ProductProductionShift
  (renamed: ProductTitle←ProductName, ProductProductionLineTitle←fld_ProductPropertyATitle, ProductProductionShiftTitle←fld_ProductPropertyBTitle)
**GetAllProductPropertyAData** → fld_ProductPropertyAId, fld_ProductPropertyATitle
  (renamed: ProductionLineCode←fld_ProductPropertyAId, ProductionLineTitle←fld_ProductPropertyATitle)
**GetAllProductPropertyBData** → fld_ProductPropertyBId, fld_ProductPropertyBTitle
  (renamed: ProductionShiftCode←fld_ProductPropertyBId, ProductionShiftTitle←fld_ProductPropertyBTitle)
**GetAllUser** → Id, Username
  (renamed: UserId←Id, UserName←Username)

---

## 5. Data model (DTOs, namespace `RfidConnectRegister.DML`)

All plain classes. **Numeric-looking fields are mostly strings**; dates are strings in Shamsi/ad-hoc formats. Property lists below are verbatim from the source.

### 5.1 Core DTOs
- **`Product`** (master data): `ProductCode, ProductTitle, ProductENTitle, ProductPackValue, ProductCountInPack, ProductValue, ProductPackWeight, ProductPackVolume, ProductTechnicalCode, ProductProperties, ProductType, ProductStatus, ProductSize, ProductSizeTitle, ProductUnit, ProductBrand, ProductGroup, ProductTechnicalData, ProductGroupTitle, ProductBrandTitle, ProductStatusTitle, ProductTypeTitle, ProductRegUser` (all `string`). `ProductTechnicalData` = JSON object of display attributes (shown read-only in F2 and passed to label templates). `ProductProperties` = JSON of dynamic-field values.
- **`Print`** (one print row): `int Id; string ProductSerial, ProductOldSerial, ProductTitle, ProductENTitle, ProductTechnicalData, RegisterDate, RegisterUser, ProductCode, ProductDescription; decimal ProductCount; string TagEpc; int PrintFlag, RegisterFlag, RegisterType; string InputFileName, PrintType, PrintUser, ErrorTime, ErrorDesc; int PrintActionId; DateTime PrintActionDateTime; string ProductUnit; decimal ProductItemCount, ProductCountInPack; string ProductSize, ProductSizeTitle, ProductRegCode; decimal ProductWeight, ProductVolume; string ProductStatus, ProductBrand, ProductGroup, ProductBrandTitle, ProductGroupTitle, ProjectCode, ProductStatusTitle, ProductProductionShift, ProductProductionShiftTitle, ProductProductionLine, ProductProductionLineTitle, ProductContractType, ProductType, DocumentId, ProductTypeTitle, WareHouseCode, ProductProperties`.
- **`Tag`** (a registered item = product + registration + package + last movement, flat): `ProductSerial, ProductOldSerial, ProductCode, TagEpc, ProjectCode; decimal ProductCount; string ProductName; int ProductType; string NotEnterCount, ProductTypeTitle, ProductStatus, ProductStatusTitle; int TagStatus; string TagStatusTitle, TagRegisterShamsiUnixDate, TagRegisterUser, TagTreeParentId, TagTreeSecondParentId, TagTreeParentsId, NewProductSerial, ProductProperties; bool Lock, Freeze, Deactivate; int TagInActionId; string TagInDestinationId; int TagInActionId2; string TagInDestinationId2, fld_ProductPropertyAId/BId/CId, fld_ProductPropertyATitle/BTitle/CTitle, RegCode, ContractStatus, TagZone, ProductUnit; DateTime TagRegisterDateTime; string FreezeStatusTitle, InspectStatusTitle, ProductValue, ProductPackWeight, ProductPackVolume, ProductCountInPack, ProductSize, TagTreeParentSerial, TagTreeParentsEpc, RActionId/Date/Time, HActionId/Date/Time/Data, HMovementActionCarPlaque, HMovementActionDriverName`.
- **`DynamicFieldDto`**: `int Id; string Title; DynamicFieldType FieldType (ItemData|HeaderData); bool IsSystematicField, IsHeaderKey; string UserName, UserId; DateTime DateTime; string RelatedTitle1/2/3; int? ActionType; bool FieldShowColumn, FieldShowColumnForAction; DynamicFieldValueType ValueType (TextBox|DropDown|RichTextEditor|Numeric); string ValueOptions, DefaultValue; bool? IsRequired; int? SectionId, Order, ParentId; string Value`. **The client only reads `title`, `id`, `relatedTitle1` from the server** and decides the control by `RelatedTitle1` (comma-separated option list → drop-down, else text box). Other members are unused.
- **`WarehouseDto`**: `int Id; string DestinationCode, DestinationTitle; DestinationOperationalType OperationalType (NotSpecified=-1, Production, Product, Material, Waste, Loading); DestinationInventoryType InventoryType (NotSpecified=-1, Virtual, Physical); bool IsDefault, allowCancel, allowRePrint, IsActive; string Epc`. **Server mapping (`SGetAllWarehouses`):** JSON keys `destinationCode, destinationTitle, operationalType (int), inventoryType (int), isDefault, isActive, id, Epc`, plus `epc` = a **JSON string** holding a dictionary whose keys `AllowCancelTag` / `AllowRePrintTag` (`"true"`/`"false"`) become `allowCancel` / `allowRePrint` (missing/empty → false). The hub offers only warehouses with `OperationalType == Production && InventoryType == Physical`; cancel is allowed only when the tag's current destination has `allowCancel`.
- **Lookups:** `ProductStatus{ProductStatusCode, ProductStatusTitle}`, `ProductType{ProductTypeCode, ProductTypeTitle}`, `ProductPropertyC{ProductPropertyC_Id, ProductPropertyC_Title}` (= product **size** list), `ProductBrand{Code,Title}`, `ProductGroup{Code,Title}` (group = "شانه" in the UI), `ProductSize`, `ProductionLine{ProductionLineCode, ProductionLineTitle}` (server name PropertyA), `ProductionShift{ProductionShiftCode, ProductionShiftTitle}` (server name PropertyB), `User{UserName, UserId, UserType}`.
- **Documents** (`Document.cs`): `DocumentHeader`(Id, Key, UserId, ImportType {Excel, Api, Manual, Other=1234}, FileName, DocumentType, ImportDateTime, Description, Status, HeaderData, DocumentItems), `DocumentItem`(Id, Key, DocumentType, ProductCode, ProductTitle, Count, Remain, ProductUnit, ItemData, StatusDesc), `GetAllAggDocDto`(DocumentKey, DocumentType, ImportDateTime?, ItemCount, ItemSum, DocumentData). A "document" is a **production plan / roll code** whose items carry requested quantities.
- **Other (unused by the screens read):** `EnterRequest`, `MovementAction`, `Zones`.

### 5.2 `Print` flags
`PrintFlag` 0 = not printed, 1 = printed. `RegisterFlag` 0 = not registered, 1 = registered. `TagEpc` non-empty = a tag is bound. `ProductContractType` = `"0"` ("ReProduct") for new batches.

### 5.3 JSON blobs stored inside DTOs
- `ProductTechnicalData` / `ProductProperties`: JSON objects `{ "<title>": "<value>" }`. Dynamic-field values are saved as `ProductProperties` of the print row and re-read by the print forms; **every key becomes a label-template variable** (§9).
- Document `headerData`: a JSON-like string parsed by naive string splitting (F2).

### 5.4 `GetRegisterRequestFilter` (parameter `filter` of the three report RPCs)
`ProductCode, Line, FromDate, ToDate, User, Shift, Qc, TechnicalCode, ProductSerial, ProductOldSerial, bool TechnicalCodeLike=false, Size="-1", ContractStatus, ProductGroup, ProductBrand, RegisterDevice="-1", InspectStatus, FromTime, ToTime` — all strings; "no filter" is the literal `"-1"`. Exact value rules in F14.

---

## 6. Domain glossary and terminology traps

| Term | Meaning in this system |
|---|---|
| Serial (`ProductSerial`, "سریال محموله") | the printed barcode id of one pack/pallet/roll; the unit of registration. `ProductOldSerial` ("سریال قدیمی") = the operator-supplied / pre-generated serial. |
| EPC / tag id (`TagEpc`, "شناسه تگ") | the 24-hex-char (96-bit) id of a UHF RFID tag; all readers produce exactly 24 chars. |
| Register ("رجیستر") | bind a serial to an EPC on the server (`SRegisterTag`). "Cancel" ("ابطال") is a soft delete of that binding. |
| Print action / PrintActionId ("کد عملیات") | batch id grouping print rows created by one save; allocated by `SGetNewPrintActionId`; navigable with "<" / ">". |
| Roll code / document id ("کد رول"/"شماره سند") | production-plan document; default `"000"` = none. |
| Production line ("خط تولید") | server "PropertyA". **The UI also calls it "سالن تولید" (production hall)** in some places. |
| Shift ("شیفت تولید") | server "PropertyB". |
| Product type ("سالن تولید" in master data, "نوع کالا") | **same Persian words "سالن تولید" mean *ProductType* in F3/F4** and *production line* in F2/F14 — keep the operator-facing words but model them separately. |
| "رنگ کالا" (colour) | the product **status / quality grade** (`ProductStatus`; sample values `1, 2, 3, SPC, اختلاطی 6/7/8, دپو`; in F2 labelled "کنترل کیفی"). |
| "سایز کالا" (size) | `ProductPropertyC` list (ids like `60*60`). |
| "کد طرح" (design code), "کد تولید/کد فنی" | `ProductTechnicalCode` = `RegCode` = `ProductRegCode`. |
| "شانه" / "برند" | product group / brand. |
| "مقدار محموله" (pack value) / "متراژ" | quantity of one pack (square metres in the tile deployment, unit `متر مربع`); "مقدار کالا" = `ProductValue` (per carton); "تعداد کالا در محموله" = `ProductCountInPack`. |
| "رول"/"بسته" | a parent package grouping several serials (`SetParent`). |
| "بازرسی" (inspection), "انبارهای تولید" (production warehouses) | `InspectStatusTitle` "بازرسی نشده" = not inspected; `TagStatusTitle` "انبارهای تولید" = still in production warehouses (string-compared in F10). |
| "بارفید" (barfid) | bar-code + RFID pairing flow (F8). |

---

## 7. Form specifications

Each form lists: purpose, entry/exit, controls, load sequence, event-by-event behaviour with exact messages, API calls, rules and gotchas. Message strings are verbatim Persian with gloss.

### F1. `FrmLogin` (`UI/FrmLogin.cs`) — entry form

**Entry point:** `Program.Main` runs `GetSettingDictionary.GetSetting()`, sets `RestApi.Ip = Settings.ServerIP`, then `Application.Run(new FrmLogin())` (Program.cs:~21-95). Global handlers `Application.ThreadException` / `AppDomain.UnhandledException` just show `MessageBox(ex.ToString())`. Licensing (`RegisteryClass.CheckRegistery`), auto-start registry key, and an older `FrmParent`/`FrmRegister` navigation are all **commented out** in Program.cs.

**UI controls:** `txtUserName`, `txtPassword`, `button1` (login), `button2` (exit → `Application.Exit()`), `btnVisiblePanelIp` (gear → opens `FormSetting` modally), `pnlIp` (hidden panel) containing `txtIp` + `btnSaveIp`.

**Behavior**
| Trigger | Steps |
|---|---|
| Enter key (form KeyDown) | moves focus to the next control (`SelectNextControl`) — not a submit |
| Load | `txtIp.Text = DicSetting["ServerIP"]` (swallows any exception) |
| `button1` click (login) | `ApiBusiness.Login(username.TrimEnd(), password.TrimEnd())` (RPC `TSLogin`). **false** → warning box "نام کاربری و یا رمز عبور اشتباه است" (wrong username/password). **true** → `Items.OnlineUserName = txtUserName.Text`; then if `Settings.NotifApp`: hide form, `new TaskTrayApplicationContext()`, `SchasualedNotifCreator.StartschasualedNotifCreator()`; else `new frmStoreRegisterTags().Show()` (the login form stays open but un-hidden in the non-tray path) |
| `WebException` / `AggregateException` containing a `WebException` | box "اتصال به سرور ممکن نیست" (cannot connect to server) and `pnlIp.Visible = true` |
| `txtIp` TextChanged | if any char is not `0-9` or `.` the whole text is cleared |
| `btnSaveIp` click | `DicSetting["ServerIP"] = txtIp.Text`; `SaveSettingInFile()`; `RestApi.Ip = …`; hide panel. On exception: "در ثبت آی پی مشکلی بوجود آمده است". **Note:** this writes only the file-backed dictionary, **not** `Properties.Settings.Default.ServerIP`, which is what `ApiBusiness` reads when building its URL (see Gotchas) |

**Gotchas**
- `GetLocalIpAddress()` is called on load but its result is discarded.
- The server IP is typed as `host:port` in practice (default `localhost:97`), yet `txtIp` rejects every non-digit/non-dot character, so a port number cannot be entered through this panel (the `FormSetting` screen has to be used).
- Login result state is only `Items.OnlineUserCode/Name/RealName`; there is no token. Closing the main form (`frmStoreRegisterTags.FormClosed`) calls `Application.Exit()`.

---

### F2. `frmStoreRegisterTags` (`UI/StoreProduct/frmStoreRegisterTags.cs`, 1936 lines) — main hub: define a print/registration batch

**Purpose.** The operator picks a product, production line/shift/store, a "roll code" (document id) and a count; the form creates N *print rows* on the server (one per serial) under one `PrintActionId`, then opens the print form. It is the navigation hub for every other screen.

**Opened by:** `FrmLogin` (non-tray mode) or the tray menu. **Closing it exits the whole application** (`FormClosed` → `Application.Exit()`).

**State fields:** `_listProduct`, `_listProductionLine`, `_listProductionShift`, `_listProductStatus`, `_listWareHouses` (loaded once on Load); `_choosedProduct` (the selected `Product`); `_NotsavedPrintList` (serials prepared client-side, not yet saved); `_savedPrintList` (rows of an existing action id); `_listOfRemainProductByDocumentId` (DataTable Count/ProductCode); `_documentItem` (items of the selected document with Count/Remain); `_dynamicFieldDtos` (dynamic field definitions + current values); `GetDocumentDataFlag` (document already loaded).

#### UI inventory (Persian label → control)
| Label (gloss) | Control | Notes |
|---|---|---|
| کد کالا (product code) | `txtProductCode` MaskedTextBox, **read-only** | filled by picker (`pictureBox9` opens `frmSearchProducts`) or Enter key |
| عنوان کالا (title) | `txtProductTitle` | read-only |
| سایز کالا (size) | `txtProductSize` | read-only, shows `ProductSizeTitle` |
| کد طرح (design/reg code) | `txtProductRegCode` | read-only, = `ProductTechnicalCode` |
| مقدار کالا (value) | `txtProductValue` | hidden, read-only |
| تعداد در محموله (count in pack) | `txtProductCountInPack` | editable; Leave → recompute pack value |
| متراژ (pack value / metrage) | `txtProductPackValue` | editable |
| وزن محموله / حجم محموله (pack weight/volume) | `txtProductPackWeight` (hidden), `txtProductPackVolume` | read-only |
| واحد کالا (unit), `txtProductENTitle`, `txtProductType` | read-only; unit and EN title hidden |
| خط تولید (label text shows "سالن تولید" = production hall) | `cmbProductionLine` | bound to `ProductionLine` list |
| شیفت تولید (shift) | `cmbProductionShift` | |
| انبار تولید (production store) | `cmbStore` | warehouses filtered (see rules) |
| کنترل کیفی (QC status) | `cmbProductStatus` | disabled by default; auto-selected from product |
| شماره سند / کد رول (document / roll code) | `txtDocumentId`, default `"000"` | `pictureBox1` opens document-info panel; `btnSearchDoc` opens document list |
| تعداد برگه (number of sheets/tags) | `txtCount` | read-only and fixed `"1"` when `RegisterCountEvryTimeIsOne` |
| کد عملیات (action id) | `lblPrintActionId` + `btnNextActionId` "<" / `btnPreviousActionId` ">" | |
| `lblPrintActionStatus` | hidden status flag "0"/"1" (1 = saved) | |
| Buttons | `btnSave` "ثبت (F6)", `btnNew` "جدید (F5)", `btnPrintRegisterTags` "چاپ لیبل >> (F7)", `btnGetSerialsList` "دریافت لیست", `btnSetParrents` "گروه بندی", `button5` "تنظیمات", `btnCancelRegister` "ابطال رجیستر تگ", `btnRepCancelRegisterTag` "تست رجیستر تگ", `btnRepRegisterTag` "گزارش رجیستر تگ ها"; hidden: `btnChangeRegisterDate` "تغییر تاریخ ثبت", `button10` "گزارش خطاهای رجیستر", `button11` "تست رجیستر تگ", `button2/3/4`, `btnDefineProductType`, `btnDefineProduct` |
| Panels | `pnlDynamicFeilds` (dynamic fields), `pnlProductTechnicalData` (read-only product tech data), `pnlGetSerialList` (3 tabs: "معرفی بازه سریال ها" range / "دریافت فایل Excel" / "دریافت از طریق Api" — the API tab has `button16` with **no handler logic**), `pnlDocs` (document list), `pnlDocumentInfo` (header+items of the document), `pnlMultiProductInOnePrintActionId` (multi-product grid), `pnlComputeConvertWeightToProductPackValue`, `lblWaiting` ("سامانه در حال دریافت اطلاعات از سرور می باشد — wait…") |

Grids: **`dgvMultiProductInOnePrintActionId`** columns: کد کالا, کد طرح, شرح کالا, تعداد تگ (`dgvMultiCount`), [hidden ItemCount], تعداد کالا در هر تگ (`CountInPack`), متراژ هر کالا (`dgvMultiProductCount`), [hidden FlagSave "0"/"1"], متراژ کل تگ (`Column1`), کد سند, delete button. **`dgvDocsList`**: کد سند, [hidden] توضیحات سند, انتخاب (select). **`dgvDocumentItems`**: ردیف, کد کالا, عنوان, مقدار درخواست (requested), باقی مانده (remaining), select button. **`dgvDocumentHeader`**: عنوان / مقدار. **`dgvTempSerialList`**: سریال.

**Keyboard:** F5 = clear all; F6 = save; F7 = open print form (only when `lblPrintActionStatus == "1"`; F7 does *not* include `KindRegister == 3`, unlike the button).

#### Load sequence
1. `lblWaiting` docked fill; clock timer on; `lblUserName = Items.OnlineUserName`.
2. `GetAndFill()`: RPCs `SGetAllProductsDefined`, `GetAllProductPropertyAData` (lines), `GetAllProductPropertyBData` (shifts), `SGetAllProductStatus_winapp`, `SGetAllWarehouses`; binds combos. **For each of status/line/shift/store combo: if exactly one item → select it and disable the combo, else `SelectedIndex = -1`.**
3. `GetANDSetDynamicFieldDtos()` → RPC `SGetDynamicFieldsByActionTypeId(actionTypeId=0)`; builds one label + one input per field, ordered by `Id`: if `RelatedTitle1` split on `,` has **more than 1 element → ComboBox with those items**, else **TextBox**. Controls are named `"dynamic_Control"+Title`, `Tag = field.Id`; `Leave` event copies text into `_dynamicFieldDtos[id].Value`. Layout switch: ≤2 fields → small panel at bottom; >2 fields → tall side panel (hidden in multi-product mode).
4. `Clear(0)` then settings-driven UI: if `MultiProductInOnePrintActionId` → hide `btnNew`, `btnSave.Text = "اضافه کردن به لیست"` (add to list), hide `btnGetSerialsList`, show multi panel; if `ComputeAverageProductPackValueFromAllWeight` → `btnGetSerialsList.Text = "وزن میانگین"` and visible; if `!SetParentForSerials` → hide `btnSetParrents`.

`Clear(kind)`: **0** = new action (also calls RPC `SGetNewPrintActionId` → `lblPrintActionId`, status "0", clears dynamic fields/values, multi grid, hides `btnGetSerialsList` when `GetListSerial=="0"` unless weight mode) **then falls through** to kind 1; **1** = clears serial list, product fields, sets `txtDocumentId="000"`, `txtFromSerial/ToSerial="0"`, `txtDeleteCharFrom="0"`, `txtDeleteCharTo="4"`, count = "1" read-only if `RegisterCountEvryTimeIsOne` else empty; resets combos (single-item combos selected; shift auto-set via `GetAutoShift()` when `AutoDetecteShift=="1"`); **2** = clears product fields only (used after adding a row to the multi list).

#### Event behavior
**Select product** (Enter in `txtProductCode`, picker, or document-item "select" button) → `FillProductLableInfo(code, regCode, status)`:
1. If a document is loaded (`GetDocumentDataFlag && _documentItem.Count>0`) and the code is not among its items → box "کالای انتخاب شده با اقلام برنامه تولید تعیین شده تطابق ندارد" (selected product does not match the production plan items) and abort.
2. Find the product in `_listProduct` by `ProductCode`; if found copy: CountInPack, ENTitle, PackValue, PackVolume, PackWeight, Size (title), Status → `cmbProductStatus.SelectedValue`, RegCode (=TechnicalCode), Title, Unit, Value, Type, and render `ProductTechnicalData` JSON as read-only text boxes `"name : value"` in `pnlProductTechnicalData`. If not found → `ClearProductLableInfo()`.

**Pack-value computation:** `txtProductCountInPack.Leave` and `bntComputeProductPackCount` set `PackValue = CountInPack × ProductValue` (decimal) when both non-empty.

**`btnSave_Click` (F6)**
1. `CheckUserAccessSavePrint()`: if `HaveLogin` → username (case-insensitive) must be in a **hardcoded list of 12 usernames** (file:807); otherwise everyone allowed. Fail → "کاربر گرامی شما مجاز ثبت سریال جدید نمی باشید".
2. `CheckSavePrintData()` — sets `errorProvider1` messages (and returns false) when: line not selected ("خط تولید باید مشخص شود"), shift ("شیفت تولید باید مشخص شود"), store ("انبار تولید باید مشخص شود"), product code empty ("کد کالا باید مشخص شود"), count "" or "0" ("تعداد باید مشخص شود"), document id "" or "0" ("کد رول باید مشخص شود" — note the default `"000"` passes), QC status ("کنترل کیفی کالا باید مشخص شود"; checked twice).
3. Yes/No confirmation: "آیا از ثبت اطلاعات اطمینان دارید؟ شما در حال ثبت تعداد: {count} سریال از کالا با عنوان {title} با کد تولید {regcode} و با کد رول {document} می باشید".
4. **Single-product mode** (`!MultiProductInOnePrintActionId`): `CheckProductCodeANDCountByDocItems(code, count × (CountInPackEqualCount ? CountInPack : 1))`. If `CheckRegisterByDocItems` is on and `_listOfRemainProductByDocumentId` has a row for the product whose `Count` is less than the requested number → error "تعداد وارد شده بیشتر از مقدار باقی مانده در سند تولید می باشد" and abort. Otherwise `SavePrint(...)`, then ask "ثبت با موفقیت انجام شد ، آیا قصد چاپ لیبل ها را دارید؟" → on Yes behave as `btnPrintRegisterTags_Click`.
5. **Multi-product mode:** same check using `count × CountInPack` (always multiplied here); on pass append a row `[code, regCode, title, count, value, countInPack, packValue, "0"(flag), round(count×packValue,2), documentId]` to `dgvMultiProductInOnePrintActionId`, recompute `lblCountMultiProductInOnePrintActionId` (sum of column 3), `Clear(2)`, and refresh remaining counts via `SGetListOfRemainProductByDocumentId`. `btnConfirmMultiProductInOnePrintActionId` ("تأیید لیست و چاپ تگ ها") then calls `SavePrint` for all unsaved rows; `btnCancelMultiProductInOnePrintActionId` clears everything.

**`SavePrint` → RPC `SSavePrintBySerial`** (via `ApiBusiness.SavePrintBySerial(Print, printCount, productPropertiesJson)`; see API section). Three branches:
- *Multi mode*: for each grid row whose hidden flag is "0": builds a `Print` with empty title/size/unit/status/type, `ProductItemCount`/`CountInPack`/`ProductCount` from the row, `PrintCount` argument = row's `dgvMultiCount` ("تعداد تگ"), then flags the row "1". The `await` is applied.
- *Single mode with serial list* (`_NotsavedPrintList.Count != 0`): **one RPC per serial** with `printCount = "1"`, `ProductSerial = ProductOldSerial = the serial`. **Not awaited** (fire-and-forget; success/failure ignored).
- *Single mode without list* and `txtCount` ≠ ""/"0": one RPC with `ProductSerial=""`, `printCount = txtCount` → the **server generates the serials**.
- All branches set: `PrintActionId`, `PrintFlag=0`, `RegisterFlag=0`, `PrintUser = Items.OnlineUserCode` (the user **id**, not name), `ProductContractType = "0"` (ReProduct), `DocumentId` ("0" if empty), `WareHouseCode = cmbStore.SelectedValue`, brand/group from `_choosedProduct`, and `ProductProperties` = JSON object `{ "<dynamic field Title>": "<value>", … }` built from `_dynamicFieldDtos`. Afterwards `lblPrintActionStatus = "1"`.

**Serial-list panel (`btnGetSerialsList` → `pnlGetSerialList`)**: prerequisites checked first (product code, line "سالن تولید", shift, action id) else box "لطفا ابتدا اطلاعات زیر را تکمیل نمایید". In weight mode (`ComputeAverageProductPackValueFromAllWeight`) the button instead opens the *average-weight* panel (requires product code, count ≠ 0/"", pack value ≠ 0/""): user enters `txtAllWeightForConvert`; average = `round(allWeight / count, 2)`; Confirm copies it into `txtProductPackValue`.
- *Range tab* (`btnComputeSerialsList`): if both "delete char" boxes are "0": `From`/`To` parsed as int. Else `DeletedChas = From.Substring(deleteFrom, deleteTo)`, and both numbers are `Remove(deleteFrom, deleteTo)` then parsed. Rows generated for `i = From … To-1` (**To is exclusive**) as `DeletedChas + i.PadLeft(len(txtFromSerial) - len(DeletedChas), '0')`; `txtGetSerialListCount = To - From`. (Default `deleteTo="4"` after Clear.)
- *Excel tab* (`btnGetExcel`): `OpenFileDialog` (.xls/.xlsx) → `AvizheExcel.ExcelReader.ReadExcelDataExportInDataTable` → first column of every row becomes a serial; filename shown in `lblExcelFileName`.
- *Save* (`btnGetSerialListSave` "تأیید"): each grid row → `Print{PrintActionId, ProductCode, ProductCount = txtProductPackValue, line, shift, ProductSerial = ProductOldSerial = row value}` added to `_NotsavedPrintList`; `txtCount = number of rows`; button turns green.

**Document ("roll code") flow.** `btnSearchDoc` → `GetAggDocsByDocTypeAndStatus("1")` (RPC `SGetDocumentsByDocType`, documentType "1") fills `dgvDocsList` (key, `headerData`, button "انتخاب"); typing in `txtSearchDocInlist` hides rows whose *key* does not contain the text (note: only filters when text non-empty, so clearing the box does not un-hide). Selecting a row → `txtDocumentId = key` → `GetDocumentData(key)`: RPC `SGetDocumentData(documentKey, documentType=1)`; empty result → "اطلاعاتی برای کد سند : … یافت نشد". Otherwise `headerData` (JSON) is parsed **by splitting each property's string on ':'** and stripping quotes/braces/`\r\n`/backslashes; each header `title → value` is shown in `dgvDocumentHeader` **and auto-filled into the dynamic-field control/DTO of the same title if that control is still empty**. `documentItems` JSON → `_documentItem` list (`productCode`, `productTitle`, `count`; duplicates summed), shown with a select button. Then `SGetRemainRegisterByDoc(DocumentId)` fills remaining quantities (`Remain`); counts in the multi grid not yet saved (flag "0") for the same doc/product are subtracted (`count×countInPack` if `CountInPackEqualCount`, else `ProductCount`). Selecting an item row fills `txtProductCode`, runs `FillProductLableInfo`, and sets `txtCount` to the item's remaining quantity. Changing `txtDocumentId` to ""/"0"/"00" resets all document state.

**Action-id navigation:** "<" button (`btnNextActionId`) → id + 1 and reload; ">" button (`btnPreviousActionId`) → id − 1 (min 1) and sets status "1". Reload = RPC `SGetPrintListByPrintActionId`; fills `dgvTempSerialList` with `ProductOldSerial`s, `txtCount` = row count, product code from the first row, then product info.

**Open print form** (`btnPrintRegisterTags`, `button9`): allowed when `lblPrintActionStatus == "1"` **or** setting `AllowShowOnlyPrintFormBeforeSavePrintAction`; else "لطفا ابتدا عملیات را ثبت نمایید". `KindRegister == "1"` → `frmPrintRegsiterTags(actionId, count)` (count = `txtCount`, or multi total); `"2"|"3"|"4"` → `frmOnlyPrint(actionId)`. (`button9` opens form 1 modally, others non-modally.)

**Other buttons:** "تست رجیستر تگ" → `KindRegister == "2"` ? `frmTestRegister` : `frmTestRegisterGroup` (modal); "ابطال رجیستر تگ" → `frmCancelRegisterTag`; "گزارش رجیستر تگ ها" → `frmRepRegisterTag`; hidden ones → `frmRepCancelRegister`, `frmRepErrorInRegisterTag`, `frmChangeTagRegisterData`, `frmDefineProducts`, `frmDefineProductType`; "گروه بندی" → `frmSetParent`; "تنظیمات" → `FormSetting`.

**Hardcoded rules / gotchas**
- `GetAutoShift()` returns the *string* "SHIFT 1" for 06:00–13:59, "SHIFT 2" for 14:00–21:59; the "SHIFT 3" branch (`Hour < 6 && Hour >= 22`) is **never true**, so 22:00–05:59 yields `""`. It is applied by assigning `cmbProductionShift.Text`, so it only works if the server's shift titles are literally "SHIFT 1/2".
- `cmbStore` filter is `p.IsActive = true && OperationalType == Production && InventoryType == Physical` — **`=` is an assignment, not a comparison** (mutates every warehouse's `IsActive` to true; effectively ignores `IsActive`).
- `GetNewPrintActionId` swallows all exceptions; `Clear(0)` is `async` and never awaited.
- Single-list save loop does not await and ignores failures; there is no rollback and no verification of how many rows were stored.
- `lblDate.Text.Replace("/","").Substring(2,4)` result is unused (dead code).
- Dead handlers abound (`timerEpc_Tick`, many empty `*_Click`); the commented constructor shows the form once drove a "6061" desk reader directly.
- Document header parsing relies on string splitting of JSON on `:`; values containing a colon (e.g. times) are truncated to the segment after the first colon (`info[1]`).

---

### F3. `frmDefineProducts` (`UI/StoreProduct/frmDefineProducts.cs`) — product master data CRUD

**Opened by:** hub button `btnDefineProduct` (hidden by default in the hub's designer) — `new frmDefineProducts().Show()`.

**UI.** Left: grid `dgvPrintQueue` (columns "کد کالا" code, "عنوان" title, "رنگ کالا" — the third column is declared but only code+title are filled). Right form (all `MaskedTextBox` unless noted):
| Label (gloss) | Control | Maps to `Product` field |
|---|---|---|
| کد کالا (code) | `txtProductCode` | ProductCode (key) |
| شرح کالا فارسی (local title) | `txtProductTitle` | ProductTitle |
| شرح کالا لاتین (Latin title) | `txtProductENTitle` | ProductENTitle |
| کد تولید (production/technical code) | `txtProductTechnicalCode` | ProductTechnicalCode |
| واحد کالا (unit) | `txtProductUnit` | ProductUnit (reset to **"متر مربع"** = square metre after every clear) |
| سایز کالا (size) | `txtProductSize` **ComboBox** (bound to `GetAllProductPropertyC`, value = `ProductPropertyC_Id`) | ProductSize |
| رنگ کالا (colour) | `cmbProductStatus` ComboBox (bound to `GetAllProductStatus`) | ProductStatus (i.e. QC/colour status) |
| سالن تولید (production hall) | `cmbProductType` ComboBox (bound to `GetAllProductType`) | ProductType |
| مقدار هر کارتن (value per carton) | `txtProductValue` | ProductValue |
| تعداد کارتن در محموله (cartons per pack) | `txtProductCountInPack` | ProductCountInPack |
| مقدار محموله (pack value) | `txtProductPackValue` | ProductPackValue |
| وزن / حجم محموله | `txtProductPackWeight`, `txtProductPackVolume` | ProductPackWeight / ProductPackVolume |
Buttons "ثبت" save, "حذف" delete, "جدید" new, "جستجو در نوسا" (search in the WMS catalogue; panel `pnlSerachInWMS` with `dgvCusWMS`, filters `txtSearchCode`/`txtSearchTitle`).

> **Terminology warning for a rewrite:** the UI labels do not match the data names. "سالن تولید" (production hall) is used both for `ProductType` (here and in `frmDefineProductType`) and for the production *line* in the hub; "رنگ کالا" (colour) is the product *status*; "سایز کالا" (size) comes from the `PropertyC` list. The new UI should keep the operator-facing words but model them explicitly.

**Load:** `GetAndFill()` → RPCs `SGetAllProductsDefined` (grid + `_list`), `SGetAllProductStatus_winapp`, `SGetAllProductType`, `SGetAllProductPropertyC` (all with `haveNotSelect=false`), then `ClearItems()`.

**Events**
- *Select a grid row* (only when the grid has focus — `_flagSelectionChange` is set on grid Enter/Leave): `ClearItems()` then `FillData(product)` copying every field to the controls (while a "please wait" panel disables the form).
- *Size ComboBox change* — **hardcoded tile presets**: size id string `"75*75"` → value 1.69, count-in-pack 40, pack value 67.6; `"60*60"` → 1.44 / 72 / 103.68; `"60*120"` → 1.44 / 60 / 86.4; all three set pack weight and volume to "0". (This only fires when the selected *value* equals those strings, i.e. when the size master data uses the size label as id.)
- *Save:* `CheckBeforeSave()` marks "فیلد نمی تواند خالی بماند" (field cannot be empty) on code, title, pack value, size (not selected), type (not selected), status (not selected). Then `SaveProduct` (RPC `SaveProduct`) with `ProductRegUser = Items.OnlineUserCode`; unselected combos are sent as "-1". Success → "اطلاعات با موفقیت ثبت شد"; failure → "در انجام عملیات مشکلی بوجود آمده است". Then re-load and clear. The API returns only bool (create-or-update by code; the client cannot tell which).
- *Delete:* ignored if code empty; RPC `SDeleteProduct(ProductCode)`; no confirmation dialog.
- *WMS catalogue search* (`button1`): RPC `SGetCusProducts` → rows `mat_Code`/`mat_Name`; typing in either filter box filters client-side (case-insensitive *contains*, only while that box has focus). Clicking the "انتخاب" button cell copies code → `txtProductCode`, name → both `txtProductTitle` **and** `txtProductENTitle`, and closes the panel. (Double-click handler body is commented out.)

**Gotchas:** `ClearItems()` after save runs before the async re-fill completes; no unique-code check client-side; `ProductBrand`/`ProductGroup` are never editable here although the DTO and picker use them (they must come from the server/other tooling).

---

### F4. `frmDefineProductType` (`UI/StoreProduct/frmDefineProductType.cs`) — master data "production hall / product type"

Opened modally from the hub (button hidden by default). Group title "اطلاعات سالن تولید" (production-hall info), list "لیست انواع کالا".
- Controls: `txtProductCode` ("کد سالن تولید"), `txtProductTitle` ("عنوان سالن تولید"), grid `dgvPrintQueue` (کد / عنوان), buttons ثبت / جدید (a "حذف" button exists in the designer but has **no handler**).
- Load: RPC `SGetAllProductType` → grid. Selecting a row (grid focused) fills the two boxes.
- Save: both fields required ("فیلد نمی تواند خالی بماند"); RPC `SSaveProductType(ProductTypeTitle, ProductTypeParentId = "1", ProductTypeParentsId = "", ProductTypeCode, userToken = Items.OnlineUserCode)` — **parent id is hardcoded to "1"** (flat list under root 1). Messages as in F3; list reloads.

---

### F5. `frmSearchProducts` (`UI/StoreProduct/frmSearchProducts.cs`) — reusable product picker (modal)

**Constructor:** `frmSearchProducts(MaskedTextBox targetCode, List<Product> products)` — the caller's textbox is written directly (`selectedCode.Text = code`) and `DialogResult.OK` returned. **Opened by:** hub (`pictureBox9`), `frmOnlyPrint`, `frmPrintRegsiterTags`, `frmRepRegisterTag`, `frmDefineProducts` (callers pass their cached list).

**Filters (group "فیلتر های جستجو"):** code (`txtProductCode`, **exact match**), title (`txtProductTitle`, case-insensitive contains), size (`cmbProductSize`, "سایز کالا", by `ProductSize` id), colour/status (`cmbProductStatus`, "رنگ کالا"), design code (`txtProductRegCode` ComboBox, "کد طرح", exact = `ProductTechnicalCode`), group ("شانه کالا", `ProductGroup`), brand ("برند کالا"), type ("نوع محصول" = `ProductType`). A hidden checkbox "کد بسته" + `txtRegDocument` exists but is not used in the filter. Button "جستجو" runs the search, "جدید" clears, "بروزرسانی لیست از سرور" re-fetches `SGetAllProductsDefined`.

**Result grid columns:** انتخاب (button), کد کالا, عنوان کالا, سایز کالا (`ProductSizeTitle`), کد طرح, رنگ کالا (`ProductStatusTitle`), برند, شانه کالا (group title), سالن تولید (`ProductTypeTitle`). Row click on the button column **or** double-click anywhere on the row → select & close.

**Cascading filter lists:** `GetListData(products)` rebuilds each combo's items from the *distinct* values present in the current (filtered) list (code+title pairs, skipping blanks) and restores the previous selection; so combo options shrink as filters are applied.

**Search trigger quirk:** searching runs only while `FlagChange == true`, which is set by control `Enter` events and cleared on `Leave` and after each search — i.e. only user-driven changes search, programmatic resets don't. `cmbProductGroup`/`Type` change handlers pass `cmbProductSize.Text` (the *title*) instead of `SelectedValue` (the id) as the size filter, whereas other handlers pass the id → size filtering is inconsistent when those combos are used after choosing a size.

---

### F6. `frmPrintRegsiterTags` (`UI/StoreProduct/frmPrintRegsiterTags.cs`, 1418 lines) — print labels + register tags for one print action

**Constructor:** `(string PrintActionId, int CountPrint)`; opened by the hub when `KindRegister == "1"` (button/F7; `button9` opens it modally) and by nothing else. `CountPrint` is the number of rows the hub *expects*.

**UI.** Grid `dgvPrint` (one row per print row of the action). Columns (header text → source): ☑ checkbox "..." (print selection), `Id`, "سریال محموله" `ProductSerial`, "کد کالا", "عنوان کالا", "عنوان لاتین کالا", "مقدار محموله" `ProductCount`, "مقدار کالا" `ProductItemCount`, "تعداد کالا در محموله" `ProductCountInPack`, "واحد کالا", "سایز کالا", "کد تولید کالا" `ProductRegCode`, "رنگ کالا" `ProductStatusTitle`, "کد رول" (col named `…ProductWeight`, **holds `DocumentId`**), "سریال قدیمی" (col named `…ProductVolume`, **holds `ProductOldSerial`**), "خط تولید", "شیفت تولید", [hidden PrintType, PrintUser], "وضعیت چاپ" print status text, "وضعیت رجیستر" `RegisterFlag`, "عنوان فایل" `InputFileName`, "ویرایش" button ("حذف"), [hidden ProductTypeTitle, ProductContractType, TagEpc]. Header-check `chbdgvPrint` toggles all rows (checked on Load). Counters: `lblbtnPrintCountDo` (# with `PrintFlag==1`) "/" `lblbtnPrintCount` (# rows). `progressBar1` (hidden) for the FastReport path. Side panels: **add-serial** (`button9` "چاپ مجدد سریال" → panel with `txtAddSerialToPrintList`, "اضافه کردن به لیست چاپ" / "انصراف"), **edit/delete** (`pnlShowProductInfoANDEdit`: document id, product code/title (read-only), "وزن واقعی" `txtProductPackValue`, line, shift, store, dynamic fields, buttons "ویرایش اطلاعات"/"حذف اطلاعات"/"انصراف"), shortcuts "ابطال رجیستر تگ" → `frmCancelRegisterTag`, "تست رجیستر تگ" → `frmTestRegisterGroup`.

**Load sequence**
1. `GetSavedPrinList(actionId)`: RPC `SGetPrintListByPrintActionId`; **if the row count ≠ `CountPrint` it re-queries up to 3 more times** (server rows may arrive asynchronously because the hub's list save is fire-and-forget). Each row is shown with a computed *printable* flag: `PrintStatus = true` (checked, white row, text "چاپ نشده" = not printed) unless `PrintFlag == 1` **or** `TagEpc != ""` (then unchecked, green row, "چاپ شده" = printed). If any already-printed row exists → warn "همه یا برخی از سریال های این عملیات قبلا چاپ شده است ! آیا از ادامه عملیات اطمینان دارید؟؟"; if any row already has an EPC → warn "سریال های زیر قبلا رجیستر شده اند …" listing `serial - oldSerial`. **No** closes the form.
2. `ComputeCountPrinted()`.
3. Builds the print buttons from the file system (all paths relative to the exe folder):
   - `IsZpl == true` → one button per file in `zpl_Formats\*` : text "چاپ و رجیستر تگ _ <filename without .txt>" → `PrintZplFormatAsync`.
   - else → one button per `zmin_Formats\*.lsf` (directory auto-created if missing) with the same text pattern → `PrintZminFormatAsync`.
   - **always also** one button per `Print_Formats\*` (FastReport `.frx`), button text = file name before the first `_`; the printer name is the 2nd `_`-separated token of the file name (`..._Microsoft Print to PDF.frx`, `..._0_3.frx`) → `PrintSavedFormat`.

**Row editing/deletion** (cell click on the "ویرایش" column) — allowed only while the row's `TagEpc == ""` (else "برای سریال انتخاب شده تگ صادر شده و امکان ویرایش اطلاعات آن از این طریق امکان ندارد"). *Edit* = RPC `SDeletePrintByPrintId(id)` **then** `SSavePrintBySerial` re-creating the row with the same serial, new line/shift/store/document/`ProductCount` (= "وزن واقعی"), `printCount "1"`, and the dynamic fields JSON; the grid row is replaced by re-fetching it (`SGetPrintDataByProductSerial`). *Delete* = `SDeletePrintByPrintId` only. The edit panel's dynamic fields come from `SGetDynamicFieldsByActionTypeId(0)` pre-filled from the row's `ProductProperties` JSON (matched by case-insensitive trimmed title). Note: the delete-then-insert edit is not transactional.

**Add serial to print list** (`btnAddSerialToPrintList`): RPC `SGetPrintDataByProductSerial(serial)`; rows not already in the grid are appended (checked when `PrintFlag == 0`). Used to re-print an arbitrary earlier serial.

#### Print path 1 — ZPL (`PrintZplFormatAsync`)
Pre-checks: grid empty → "هنوز هیچ موردی برای چاپ تعریف شده است"; earlier-registered serials → confirm again. For every **checked** row:
1. Requires `Settings.PrinterModel` ≠ "" else "ابتدا چاپگر را برای این نوع از جاپ مشخص کنید" (and returns — **leaving `pnlZplButtons` disabled**).
2. Reads the `.txt` template (Tag of the button) and substitutes placeholders (square-bracket tokens, Persian names):

| Token | Value |
|---|---|
| `[سریال_قدیمی]` | `ProductOldSerial` |
| `[سریال_محموله]` | `ProductSerial` |
| `[عنوان_کالا]` | `ProductTitle` |
| `[عنوان_لاتین]` | `ProductDescription` (not the EN title) |
| `[کد_کالا]` | `ProductCode` |
| `[تاریخ_تولید]` | Shamsi date of `PrintActionDateTime` (`yyyy/MM/dd`, via `Time.ConvertMiladi2ShamsiDate`) |
| `[روز_تولید]` / `[ماه_تولید]` / `[سال_تولید]` | `Substring(6,2)` / `(4,2)` / `(2,2)` of the date with `/` removed (day / month / 2-digit year) |
| `[کد_فنی]` | `ProductRegCode` |
| `[مقدار_محموله]` | **`ProductRegCode`** (copy-paste bug; the ZMIN path uses `ProductCount`) |
| `[کد_سند]` | `DocumentId` |
| `[شناسه_تگ]` | the EPC (below) |
| `[<any key>]` | every key/value in the row's `ProductTechnicalData` JSON |
3. **EPC:** the shipped code assigns a **hardcoded constant** `"000000000000000000004321"` (24 hex chars). The original formula is commented out and is the only description of the intended EPC layout: `(digits-only(ProductCode) left-padded to 12 with '0'  +  3-digit random 001-999  +  digits-only(last ≤9 chars of ProductSerial) right-padded to 9 with '0')`, left-padded to 24 and cut to 24 chars; if the code equals the 9-char serial part a random 9-digit serial part is used. (A server call `SCreateEpc(serial, productCode, refCode, epc="", type="1")` also exists in `ApiBusiness.CreateEpc` but is not called here.) **A rewrite must decide the real EPC scheme.**
4. Strip `\r\n` from the template → `ZplFinal`.
5. **Register first:** `SaveRegister(serial, epc, productCode)` → RPC `SRegisterTag(productSerial, productCode, productCount="", epcList=[epc], username = Items.OnlineUserName, deviceIp "0", deviceId "0", RegisterProperties "")`. Sleep 400 ms.
6. If registered: `ZPLHelper.PrintLabelZpl(printerName, ZplFinal)` (raw spooler job, see below) → fire-and-forget RPC `PrintUpdatePrintStatus(serial)` → set `PrintFlag=1`, row green, cell text "ثبت شده" (registered). The code then has two further nested "retry" blocks guarded by `!PrintedFlag`; since `PrintedFlag` is already true after the first success they never run (effectively **no retry**). If registration returns false nothing is printed and no message is shown for that row.
7. Sleep 500 ms between rows; re-enable panel. Any exception → "در انجام عملیات مشکلی بوجود آمده است".

**ZPL conventions seen in the shipped templates** (`bin/Debug/zpl_Formats/*.txt`): header `^XA ^PR2 ^MD25 ^PW760 ^RS60,B9`; RFID write `^RFW,H ^FD[شناسه_تگ] ^FS` (hex write of the EPC; absent from the "copy" template); logo as an inline `^GFA` bitmap; text/barcode fields `^FO x,y ^BY3 ^FD[سریال_محموله] ^FS`; footer `^XZ`. Template names: "لیبل محصول" (full label), "لیبل محصول رجیستر و چاپ فقط سریال" (only the serial barcode + RFID write).

**Raw printing (`ZPLHelper`).** `PrintLabelZpl(printer, zpl)` writes the UTF-8 text to the Windows spooler with data type `RAW` using `OpenPrinter/StartDocPrinter/StartPagePrinter/WritePrinter/EndDocPrinter` (P/Invoke, `winspool.drv`); always returns true (exceptions propagate). `PrintLabelBase64Image` converts a base64 image to 1-bpp, stores it on the printer as `R:LBLRA2.GRF` with `^XA~DG…^FS^XZ` (Z64-less ASCII hex with ZPL run-length compression: `,` all-zero row, `!` all-ones row, `:` same as previous row, repeat codes `g-z` ×20 and `G-Y` ×1-19, max 419), prints with `^XA^FO0,0^XG…,1,1^FS^XZ`, deletes with `^XA^ID…^FS^XZ`. Filename must match `^[REBA]:[A-Z0-9]{1,8}\.GRF$`. (`PrintLabelBase64Image` is not called by any form in the explored code.)

#### Print path 2 — ZMIN vendor SDK (`PrintZminFormatAsync`, used when `IsZpl == false`)
SDK = `LabelPrinter.dll` (`ZMPrinter`, `ZMLabel`, `LSFUtility`, `PrintUtility`, `LabelObject`). `printerinterface = PrinterStyle.RFID_USB`. For each checked row: `GetPrinterStatus()` (`PrintUtility.getPrinterStatusCode`; non-zero → message box and **skip row**); open the `.lsf` label (`LSFUtility.OpenLabel`); **`readEpc` is stubbed to `"000"`** (the real `GetUHFTagData` call is commented out); replace placeholders in the first variable of every label object (same token set as ZPL but `[مقدار_محموله]`=`ProductCount` and four extra: `[وضعیت_کالا]`=`ProductStatusTitle`, `[نوع_کالا]`=`ProductTypeTitle`, `[سایز_کالا]`=`ProductSizeTitle`, `[تعداد_کالا_در_محموله]`=`ProductCountInPack`; `[شناسه_تگ]`=readEpc) and technical-data keys in `objectdata`; remove label objects whose name starts with "rfid"; **register with `SaveRegister(serial, epc: serial, code)`** (i.e. the *serial string* is sent as the EPC); then `PrintUtility.PrintLabel(printer, label, objects, true, true)`; a result starting "Error:" is shown, else `PrintUpdatePrintStatus`, row green "ثبت شده". 1 s pause per row.
Printer status code → message (Persian, verbatim meaning): 86 cutter detection error; 96 label peeler waiting for label removal; 88 printer paused (press Pause to resume); 81 mainboard hardware fault; 82 carbon-ribbon detection error; 83 label detection error (calibrate paper); 89 out of label paper; 90 RFID read/write error; 91/92 RFID signal calibration failed; 99/120 firmware update finished; 1 incorrect instruction received; other → "خطای چاپگر، کد خطا:{n}".

#### Print path 3 — FastReport (`PrintSavedFormat`)
For each checked row: load the `.frx` (`Print_Formats\<name>_<printer>.frx`), set report **parameters** (Persian names): `"سریال محموله"`=ProductSerial, `"سریال قدیمی"`=ProductOldSerial, `"کد کالا"`, `"عنوان کالا"`, `"کد فنی"`=ProductRegCode, `"مقدار محموله"`=ProductCount, `"تاریخ تولید"`=`RegisterDate` (**never populated by `SGetPrintListByPrintActionId`, so null/empty**), `"کد سند"`=DocumentId, plus every key in `ProductTechnicalData` JSON; `Prepare()`. Then:
- `MergPrintToPDF == "1"`: first the `tempfastreport` folder is emptied; each row exported as `<n>.pdf` (fonts embedded); at the end `CombinePdf` merges them by numeric order with PdfSharp into `MergPdf.pdf` (exe folder) and opens it with the shell. (The `MergPdf.pdf` committed under `bin/Debug` is a build artefact.)
- else: first row → `ShowPrepared()` preview then "آیا از اطلاعات در حال چاپ اطمینان دارید ؟" (Yes → `Print()`, No → abort loop); later rows `Print()` directly (default printer/dialog, the printer-name token is only validated non-empty).
- Per-row exceptions are swallowed. **This path does not call `SRegisterTag` nor `PrintUpdatePrintStatus`** — printing with FastReport alone does not mark rows printed/registered.
Shipped templates: "برگه مشخصات محصول", "لیبل RFID فاقد لوگو", "لیبل RFID", "لیبل به همراه سریال RFID", "لیبل دوم Camera", "لیبل دوم سوئیج و NVR", "لیبل شناسه پیگیری" (the product catalogue is network/security equipment in this deployment — Camera/NVR/switch).

**Gotchas:** several `async void` handlers; `GetAllProductByTechnicalData(...)` is called inside the ZPL loop but never awaited/used; the `IsActive = true` assignment bug again in the store filter; `RFIDPrint()` and `showPrintPageData` are empty/commented stubs.

---

### F7. `frmOnlyPrint` (`UI/StoreProduct/frmOnlyPrint.cs`, 1231 lines) — FastReport printing for an existing print action (`KindRegister` 2/3/4)

**Constructor:** `(string PrintActionId)`; opened by the hub when `KindRegister` ∈ {"2","3","4"} (button, F7 for 2/4, `button9`). Same grid, edit/delete panel, add-serial panel and dynamic-field editor as F6 (same column headers; the edit button column is headed "ویرایش مشخصات", an extra "حذف" column exists; the *delete* button in the edit panel is **hidden** in this form). The not-registered panel is extra (below).

**Differences from F6**
- **Only FastReport printing** (no ZPL/ZMIN). Buttons are built **after** the list loads from `Directory.GetFiles(exe\Print_Formats)`; for each file two buttons are created: a small *preview* button and a wide *print* button (text = name before first `_`).
- **File-name convention** `<LabelName>_<PrinterName>_<BrandCode>.frx` (e.g. `لیبل دوم Camera_0_3.frx`): token[1] = printer (`"0"` → use `Settings.PrinterModel`), token[2] = brand code. If `ControlPrintFormatByBrand != "0"`, only files whose brand token equals `_savedPrintList[0].ProductBrand` get buttons (a file name with fewer than 3 `_`-tokens throws in that mode). Files in the sample: `..._Microsoft Print to PDF.frx` (2 tokens → only valid with brand control off), `..._0.frx`, `..._0_3.frx`.
- Load: single `SGetPrintListByPrintActionId` (no 3× retry, no expected count). Checked/"چاپ نشده" iff `PrintFlag == 0` (F6 also unchecks rows that already have an EPC). Warnings for already printed / already registered serials identical to F6 (No closes the form).
- If `KindRegister == "3"`: the "رجیستر تگ ها>>" button is hidden.

**Report parameters set by `PrintSavedFormatAsync` (Persian names; every name the `.frx` does not declare is silently ignored):** سریال محموله (ProductSerial), سریال قدیمی, کد کالا, عنوان کالا, کد فنی (ProductRegCode), مقدار محموله (ProductCount), سایز کالا (`ProductSizeTitle`), وضعیت کالا (`ProductStatusTitle`), واحد کالا, تعداد کالا در محموله (CountInPack), مقدار هر کالا (`ProductItemCount`), شیفت تولید (shift title), تاریخ تولید میلادی (`PrintActionDateTime`), تاریخ تولید شمسی (Shamsi date), کد سند, برند کالا, شانه کالا (group title), عنوان کالا لاتین (= `ProductDescription`), `ماه_تولید` / `سال_تولید` (Shamsi month `Substring(4,2)` / 2-digit year `Substring(2,2)`), نوع کالا, plus **every key/value of `ProductTechnicalData` JSON and of `ProductProperties` JSON** (so dynamic fields reach the template by their title). The preview button sets the same parameters (without the last three) and shows the first row only.
**Print flow:** for each checked row — report built as above, `PrintSettings.Printer = printerName`, `ShowDialog=false`, `Prepare()`. `MergPrintToPDF=="1"`: export `<n>.pdf` (embedded fonts) per row into `tempfastreport\`, finally `CombinePdf` → `MergPdf.pdf` (PdfSharp) and open. Else: first row → optional `ShowPrepared()` if `PrintPreviewBeforePrint`, then confirmation "آیا از اطلاعات در حال چاپ اطمینان دارید ؟"; Yes → `Print()`, No → stop; other rows → `Print()` directly. **When `KindRegister == "3"`, after each printed row the tag is registered immediately**: `EPC = Tools.ReplaceAllEnglishAlphbet(ProductSerial, "")` (upper-cases and strips every `A–Z`, space, `:`, `-`, `/`, `,` → leaves digits only) and `SaveRegister(serial, EPC, productCode)` → RPC `SRegisterTag` (so the serial's digits become the tag id; no physical tag is read). This form **never calls `PrintUpdatePrintStatus`**, so `PrintFlag` is not updated by it. Exceptions are shown with `ex.ToString()`.
- The preview path computes the printer token from the *Preview-prefixed* tag so it extracts the wrong token (only cosmetic; preview does not print).

**"RFID registration" button** (`btnPrintRegisterTags`, "رجیستر تگ ها>>(F7)"; kinds 2/4): picks the single serial (if exactly one row → that one; else the **first checked** row). `ActiveRegisterFormByOneSerial` = true → requires a serial ("لطفا ابتدا تیک انتخاب یک سریال را در لیست مشخص کنید"), opens `frmRFIDRegisterByBarfid(serial)` modally; on `DialogResult.OK` the row is turned green and, if `AutoPrintAfterRegister`, a second FastReport (`PrintTestAfterRegister.frx` in exe folder, printer `PrinterModelForAutoPrintAfterRegister`, no dialog) is printed with parameters `"سریال محموله"` = serial and `"RFID"` = tag EPC from `SPGetProductInfo`. `ActiveRegisterFormByOneSerial` = false → `frmRFIDRegisterByBarfid("")` (free-pairing mode).
**Not-registered list** (`btnNotRegisteredSerialList` "سریال های رجیستر نشده"): RPC `SGetRegisteredPrints` (client method `RepSGetNotRegisteredPrints`) → grid (☑ default true, serial, code, name, count); "تأیید" adds each checked serial into the print grid via `SGetPrintDataByProductSerial`. (The RPC name says "Registered" while the client says "NotRegistered" — verify server semantics.)
**Product picker** in the edit panel: `frmSearchProducts` writes into `txtProductCode`, title is looked up from `_listProduct` (the list is fetched asynchronously *after* the dialog opens → may be empty on first use).

---

### F8. `frmRFIDRegisterByBarfid` (`UI/StoreProduct/frmRFIDRegisterByBarfid.cs`) — pair a barcode serial with a physical RFID tag (register)

**Constructor:** `(string ProductSerial)`; `""` = free mode, non-empty = "single serial" mode. Opened modally from `frmOnlyPrint` (kinds 2/4). Reads `DicSetting[ReaderPort]` and `Settings.SerialLength` (default 5; field default 6).

**Controls:** `txtSerialCode` (barcode scanner wedge input), `txtEpc` (receives EPCs from the desk reader), indicator boxes `pbxBarcode`/`pbxRfid` (transparent → lime when captured), `lblShowBarcode`, `lblShowEpc`, `lblMessages`, `lblConnectToReaderStatus`, result grid `dgvRegister` ("ردیف", "سریال محموله", "شناسه تگ", "وضعیت عملیات"), buttons "ابطال رجیستر تگ" (→ `frmCancelRegisterTag`), "تست رجیستر تگ" (→ `frmTestRegister`; both stop the reader, open the dialog, restart), "مرحله قبل <<" (close). Counters `label15`/`label17`. A hidden test button calls `SaveRegister("594616","E200421914506013TESTTEST")` (dev stub).

**Start (`StartRegister`)** — message "لطفا برگه محصول را از سمتی که بارکد آن رو به بالا است نزدیک دستگاه کنید" (hold the product sheet, barcode facing up, near the device); `_isOperationOn = true`; start the reader `BllConectToDeskReader.StartIndentifyTagByDeskReader(txtEpc, null, 1)` (mode 1 = continuous single identify); status label "اتصال موفق به ریدر" (yellow) or "خطا در اتصال به ریدر" (red); enable `timerCheckFillings`; in single-serial mode the barcode is pre-filled (`CheckBarcode`), the "cancel"/"test" buttons are hidden, and the barcode-clear timer interval is set to 100 000 ms.

**Capture rules**
- `txtSerialCode` text longer than `SerialLength` is cleared; text is ignored when the operation is off.
- Enter in `txtSerialCode` → `CheckBarcode`: rejected if `barcode.Length > SerialLength`; split with `DicSetting[BarcodeSpliter]` (a single char): 3 parts → take part[2]; 2 parts → part[1]; else whole; trimmed → `_barcode`. Index/format errors show "ایندکس وارد شده در تنظیمات مقدار خارج از محدوده است" / "…درستی نیست". Barcode light turns lime; message "منتظر خواندن تگ..."; a timer clears an unmatched barcode.
- `txtEpc` change: if the operation is on and length ≤ 24 → stop the reader, store `_epcCode`, show it, light green; longer than 24 → cleared. A timer (`timerClearEPCWhenDontReadBarcode`) clears an unmatched EPC and **restarts the reader**.
- `timerCheckFillings` (periodic): when both `_epcCode` and `_barcode` are non-empty → clear inputs and call `SaveRegister(barcode, epc)`.

**`SaveRegister(serial, epc)` decision table**
1. RPC `SCheckTagEPCReturnProductSerial(epc)`; if it returns a serial (tag already bound) → confirm "شناسه این تگ قبلا برای سریال : X رجیستر شده است آیا مایل به حذف آن از سریال قبلی و اختصاص آن به سریال جدید هستید ؟"; **No → return false**.
2. RPC `SIsProductSerialExist(serial, username "", deviceIp "", deviceId "")` → status:
   - **0** (not in print table): red row "اطلاعاتی برای رجیستر این سریال یافت نشد" → false.
   - **1** (queued in print table, not registered): `SRegisterTag(serial, productCode "", count "", [epc], username = Items.OnlineUserName)`; success → green row "کالا با موفقیت رجیستر شد" (+500 ms) → true; failure → red row "خطایی در عملیات رجیستر تگ رخ داده است" → false.
   - **2** (already registered): confirm "سریال X قبلا رجیستر شده است آیا مایل به ویرایش آن با تگ جدیدی هستید ؟"; Yes → `SRegisterTag(...username "")` → light-yellow row "رجیستر تگ با موفیت ویرایش شد" / red "خطایی در عملیات ویرایش تگ رخ داده است"; **No → falls through to `return true`** (treated as success).
   - any other value → `true`.
3. On `true`: add to `_epcBarcodePair`, clear UI, `DialogResult.No`; in single-serial mode show "رجیستر تگ مربوط به سریال کالا : X با موفقیت انجام شد", set `DialogResult.OK` and close. On `false`: **stop the loop** (`timerCheckFillings` and `_isOperationOn` off) — the operator must reopen the form.

**Gotchas:** WinForms `Thread.Sleep` on the UI thread (300–1000 ms) in several places; closing stops the reader and closes the COM port; the vendor reader handle is shared through `BllConectToDeskReader` (see hardware section).

---

### F9. `frmCancelRegisterTag` (`UI/StoreProduct/frmCancelRegisterTag.cs`) — cancel a registered tag

**Opened from:** the hub and from `frmRFIDRegisterByBarfid` (button "ابطال رجیستر تگ"). Modal, no constructor args.

**Controls:** `txtEpc` (receives the EPC from the desk reader), `textBox1` (focus sink), group "اطلاعات رجیستر شده برای تگ" with labels: سریال محموله (`lblRegisterPSerial`), عنوان کالا (`lblRegisterPTitle`), کد کالا (`lblRegisterPCode`), سایز کالا (`lblRegisterPSize`), کد تولید (`lblRegisterPRegCode`), رنگ کالا (`lblRegisterPStatus`); `lblTestResult` (message), `pictureBox7` (turns red when the reader failed to start), buttons "تأیید ابطال تگ" (`btnCancelRegister`) and "انصراف" (closes the form). Timer `timerClear` = 3000 ms.

**Constructor:** reads `DicSetting[ReaderPort]`, creates `BllConectToDeskReader`, and **loads all warehouses** (`GetAllWarehouses` → RPC `SGetAllWarehouses`, list of `WarehouseDto` with `DestinationCode` and `allowCancel`).

**Load:** stop any running reader, close the COM port, `StartIndentifyTagByDeskReader(txtEpc, null, 1)` (continuous mode 1). On exception → `pictureBox7` red.

**Flow**
1. `txtEpc` text length == 24 → `GetTagInfo(epc)` (RPC `SPGetProductInfo`, param `tagEpc`).
2. If the list is non-empty **and** the warehouse list is non-empty: fill the six labels from the row whose `TagEpc == epc`; `_destinationCancelAllow = warehouse(DestinationCode == tag.TagInDestinationId).allowCancel` (**throws NullReference if no warehouse matches** — the code does not guard it).
   Otherwise: clear labels, form background red (no message text).
3. After the lookup the reader is stopped and the COM port closed (single read, then manual or timer restart).
4. "تأیید ابطال تگ": only if `txtEpc` has 24 chars. If `_destinationCancelAllow` is false → "این سریال مجاز به ابطال نمی باشد" (red). Else RPC `SCancelRegisterTag(TagEpc, username = Items.OnlineUserName, deviceId="0", deviceIp="0")`; true → "ابطال با موفقیت ثبت شد" (green); false → "خطا در عملیات ثبت ابطال تگ " (red). Then `timerClear` starts.
5. `timerClear` (3 s): reset background to white, clear labels / `txtEpc` / message, stop and **restart** the reader in mode 1.

**Gotchas:** `_destinationCancelAllow` is **not reset** when a later lookup fails, so a previous tag's permission can leak into the next cancel; no confirmation dialog besides the button; cancel is by EPC (not serial).

---

### F10. `frmChangeTagRegisterData` (`UI/StoreProduct/frmChangeTagRegisterData.cs`) — change the registration date/time of a tag

**Opened from:** the hub. Modal, no args.

**Controls:** `txtEpc`, `textBox1` (focus sink), info labels (سریال محموله, عنوان کالا, کد کالا, سایز کالا, کد تولید, رنگ کالا, وضعیت بازرسی = `lblInspectStatusTitle`, ورود به انبار = `lblTagStatusTitle`), inputs `txtNewDate` (MaskedTextBox, mask `14##/##/##`, Shamsi date) and `txtNewTime` (mask `00:00`), `lblTestResult`, buttons "تغییر تاریخ ثبت تگ" (`btnCancelRegister`, initially disabled by logic) and "انصراف". `timerClear` = 3000 ms.

**Load / form-closing:** both stop the reader, close the port and **start** the reader again in mode 1 (the closing handler also restarts it — a likely leftover bug; the reader is left running after closing).

**Flow**
1. EPC length 24 → `GetTagInfo(epc)`.
2. Found → fill labels incl. `TagStatusTitle`, `InspectStatusTitle`. **Editable only if** `InspectStatusTitle == "بازرسی نشده"` (not inspected) **and** `TagStatusTitle == "انبارهای تولید"` (production warehouses): message "امکان ویرایش تاریخ ثبت وجود دارد", button enabled, white background. Otherwise message "کالا بازرسی شده و به انبار وارد شده است و امکان ویرایش تاریخ ثبت آن وجود ندارد", button disabled, red. These are **string comparisons against server-provided Persian titles**.
3. Not found → clear labels, "تگ رجیستر نشده است", button disabled, red.
4. Reader stopped and port closed after each lookup.
5. Button → `ChangeRegisterDate()`: requires `txtEpc` length 24, serial label non-empty, `txtNewDate.Text.Length == 10`, `txtNewTime.Text.Length == 5` (**silently does nothing otherwise**, no validation message and no date-validity check). RPC `SChangeRegisterDate(ProductSerial, NewDate, NewTime)`; true → "تاریخ ثبت با موفقیت تغییر یافت" (lime) and `timerClear` starts; false → "خطایی در ثبت اطلاعات رخ داده است" (red, no clear timer).
6. `timerClear` resets the form and restarts the reader.

**Gotchas:** the date is sent exactly as typed (Shamsi, `14yy/mm/dd`) with the time `HH:mm`; the new-date/time boxes are **not cleared** by the timer.

---

### F11. `frmSetParent` (`UI/StoreProduct/frmSetParent.cs`) — group serials into a parent package ("roll"/"بسته")

**Opened from:** the hub. Modal, no args. The UI uses the words رول (roll) and بسته (package) for the same thing.

**Controls:** group "افزودن سریال به بسته بندی": `txtProductSerial` (MaskedTextBox), "+" button (`btnAddSerial`), radio `rdbBarcode` "حالت قرائت بارکد" (default; `timer_barcode` keeps focus on the serial box and the "+" button is disabled) / `rdbType` "حالت تایپ سریال" (timer off, "+" enabled); group "مشخصات بسته": `txtPackageId` ("کد بسته"), `lblEpc` (tag id of the package), `lblCount` ("تعداد سریال های بسته", rows count), `lblSavePackageMessage`; buttons "1-قرائت تگ" (`btnStartRFID`), "2-ثبت بسته" (`btnSave`), "3-چاپ لیبل" (`button2`), "پاک کردن اطلاعات و ایجاد رول جدید" (`btnClear`); grid "لیست سریال های این بسته" (`dgvSearchItems`, columns: سریال, کد کالا, عنوان کالا, مقدار, وضعیت, delete-button column "..." with text "حذف"). `txtEpc` hidden.

**Load:** stops/starts the desk reader in mode 1, sets `rdbBarcode.Checked`.

**Behaviour**
- **Add serial** (barcode mode: when `txtProductSerial` text length == `Settings.SerialLength` — typed mode: "+" button): `GetTagInfo(serial)` (RPC `SPGetProductInfo`, param `tagEpc` — the serial is passed in the EPC parameter; the server resolves either); every returned row whose serial is not already in the list is appended; the status column shows `" دارای رول با کد :" + TagTreeParentSerial` when the serial already belongs to a parent. The input is cleared.
- **Read the package's own tag** (`btnStartRFID` → reader mode 1): EPC length 24 → `lblEpc` = EPC; `GetTagListByParentCode("", epc)` (RPC `SPGetProductListByParentSerial`, params `parentSerial`, `parentEpc`) loads existing children (grid is **cleared first**), and `txtPackageId` = `TagTreeParentSerial` of the first; reader stopped afterwards.
- **Open by package code**: Enter in `txtPackageId` (non-empty, ≠ "0") → `GetTagListByParentCode(code, "")`; `lblEpc` = `TagTreeParentsEpc` of the first row.
- **Save ("2-ثبت بسته")**: only when `lblEpc` is non-empty and the grid has rows (otherwise nothing happens, no message). RPC `SRegisterNextlevelProductUnit(productUnitEpc, listEpc=[serials], userToken = Items.OnlineUserCode)` (note: `listEpc` actually carries product **serials**). Success → `txtPackageId` = RPC `GetLatestNextlevelProductUnitbyEpc` (param `EpcCode`), message "تعریف بسته با موفقیت انجام شد", package box green; failure → `txtPackageId = "0"`, message "در تعریف بسته خطایی رخ داده است", pink.
- **Remove a child** (click the "حذف" cell, column index 5): immediately RPC `SRegisterNextlevelProductUnit(productUnitEpc="-1", listEpc=[serial], userToken)` (parent EPC **"-1" = detach**), removes the row, box "حذف سریال از رول با موفقیت ثبت شد". Fire-and-forget: the result is not checked, and the message is shown regardless.
- **Print label ("3-چاپ لیبل")**: requires `txtPackageId` not in {"", "-1", "0"}, otherwise "لطفا ابتدا بسته را ثبت نمایید". FastReport template `PrintPackFormat.frx` (exe folder), printer `Settings.PrinterModel`, no dialog, direct `Print()`. Parameters: `کد بسته بندی` (package code), `تعداد اقلام` (row count), `جمع مقدار اقلام` (sum of the "مقدار" column as decimal), `لیست سریال ها` (lines `serial - title` joined with `\n`).
- **Clear** (`btnClear`): empties grid, all boxes and the in-memory list, switches back to barcode mode.

**Gotchas:** the in-memory duplicate list `_savedPrintList` is not updated when a row is deleted (a removed serial cannot be re-added until Clear); the delete handler throws on header clicks (RowIndex −1 is not guarded); `GetTagListByParentCode` catch-all swallows errors; no limit on children count.

---

### F12. `frmTestRegister` (`UI/StoreProduct/frmTestRegister.cs`) — verify that a label's barcode matches the RFID tag inside it (single)

**Opened from:** the hub and `frmRFIDRegisterByBarfid` ("تست رجیستر تگ"). No args. Reads `DicSetting[ReaderPort]`.

**Layout:** group "اطلاعات چاپ شده بر روی لیبل" (what the barcode says: serial `lblLablePSerial`, title, code, status) vs group "اطلاعات رجیستر شده برای تگ" (what the tag says: serial, title, code, size, reg-code, status). Hidden `txtEpc` (reader sink), `txtSerialCode` (barcode scanner wedge input, kept focused by `timerFocus` each 1000 ms), `lblTestResult`, `pictureBox1` (click = reset), `timerClear` 3000 ms, `timerCheckBarcode`.

**Flow**
1. Load: stop reader, close port, start reader mode 1 into `txtEpc`.
2. Barcode: any text typed in `txtSerialCode` starts `timerCheckBarcode` (debounce); on tick the serial label := `Trim()` text, `GetPrintInfo(serial, …)` (RPC `GetPrintData`, params `serials=[serial]`, `userToken=""` — the user code argument is ignored), → code/status/title fills the label side; the input is cleared. Errors are swallowed.
3. Tag: EPC of exactly 24 chars → `GetTagInfo(epc)`; fills the tag side (code, reg-code, serial, size, status title, title). If nothing returned → clear tag side, "تگ رجیستر نشده است", red, `timerClear` starts. Reader is stopped and port closed afterwards.
4. Compare (whenever either serial label text changes): if both serials non-empty → start `timerClear`; different → red + "تگ برای سریال دیگری رجیستر شده است"; equal → green + "تگ به صورت صحیح رجیستر شده است".
5. `timerClear` (3 s) / `pictureBox1` click: reset all labels, background white, restart reader mode 1.

**Gotcha (real bug):** `GetPrintData` fills `Tag.ProductStatusTitle` (from JSON `ProductStatus`), but this form reads `Tag.ProductStatus`, which is never set → a `NullReferenceException` swallowed by the empty `catch` after the *code* label is set, so the label-side **status and title stay empty**. The comparison (serial vs serial) still works, because it only uses the serials. A rewrite should show the status/title on the label side.

---

### F13. `frmTestRegisterGroup` (`UI/StoreProduct/frmTestRegisterGroup.cs`) — bulk check of many tags (group read)

**Opened from:** the hub. No args. Reads the reader port **directly from `Properties.Settings.Default.DeskUHFReader_Port`** (the others use the dictionary).

**Layout:** grid with columns شناسه تگ (hidden), سریال جدید, سریال قدیمی, کد کالا, مدل کالا (= `RegCode`), شماره سفارش (= `ContractStatus`); counters "تعداد کل تگ ها" (`lblAllTagCount`) and "تعداد تگ های رجیستر نشده" (`lblNotRegisterTagCount`, red background while ≠ "0"); status box `lblConnectToReaderStatus` (yellow = reading, green = a tag was received, red = reader failed); `pictureBox1` (reset), `timer1` 1000 ms.

**Flow**
1. Load: start reader in **mode 2** (group inventory) into hidden `txtEpc`.
2. For each EPC of 24 chars: if not already in `_detectedTags` add it with `TagStatus = -1` (= "new"); counter updated; `txtEpc` cleared.
3. Every second `Check()`: gather the tags with status −1, mark them 0, call RPC `SGetProductsByEpcs` (params `epcList`, `userToken=""`) for the **batch**; add a grid row per new tag: `(epc, serial or "رجیستر نشده", oldSerial, productCode, regCode, contractStatus)`; then repaint **all** rows (red = "رجیستر نشده", light green otherwise) and update the counters.
4. `pictureBox1` click: restart the reader (mode 2) and clear grid/list/counters.

**Gotchas:** `Check()` runs even if there are no new tags (it calls the API with an empty list each second); the red/green test compares the serial column to the literal "رجیستر نشده"; no export or save; reader errors are not surfaced except via the colour box.

---

### F14. `frmRepRegisterTag` (`UI/StoreProduct/frmRepRegisterTag.cs`, 631 lines) — registered-production report

**Opened from:** the hub. No args.

**Filters (group "فیلتر های جستجو")** — all optional **except the production line**:
| Label | Control | Type / source |
|---|---|---|
| کد کالا | `txtProductCode` + picker (`pictureBox9` → `frmSearchProducts(txtProductCode, null)`) | masked text |
| از تاریخ / تا تاریخ | `txtRegisterDateFrom` / `txtRegisterDateTo` | masked Shamsi `yyyy/MM/dd`; **default on clear: today / today+1** (`Time.ConvertMiladi2ShamsiDate`) |
| از ساعت / تا ساعت | `txtFromTime` / `txtToTime` | masked `HH:mm` |
| خط تولید | `cmbProductionLine` | RPC `GetAllProductPropertyAData(userToken="")` (title / code; "PropertyA" = production line) — **required** |
| شیفت تولید | `cmbProductionShift` | RPC `GetAllProductPropertyBData(userToken="")` ("PropertyB" = shift) |
| کاربر ثبت | `cmbUsers` | RPC `GetAllUser(userToken=Items.OnlineUserCode)` (display = value = UserName) |
| کد طرح | `txtTechnicalCode` | text |
| درجه کیفی | `cmbProductStatus` | `GetAllProductStatus` (value = code) |
| سایز | `cmbProductSize` | `GetAllProductPropertyC` (value = id) |
| سریال / سریال قبلی / کد درخواست | `txtProductSerial` / `txtOldSerial` / `txtDocumentId` | text |
Buttons: "جستجو" (search), "جدید" (clear), "Excel" (export the currently selected tab's grid), "چاپ" (print summary tab). All combos start at −1 (none).

**Search request** (`GetRegisterRequestFilter`, sent as parameter `filter`): every unset field is sent as the literal string **`"-1"`**. Mapping: `ProductCode`, `Line`, `Shift`, `User`, `TechnicalCode`, `ProductSerial`, `Size`, `ContractStatus` ← DocumentId (`"-1"` if blank), `FromDate`/`ToDate` (Shamsi with the slashes **removed**, e.g. `14030212`), `FromTime`/`ToTime` (`HH:mm` → `HHmm` + `"00"`; a bare `":"` → `"-1"`), `ProductOldSerial`, plus fixed `InspectStatus="-1"`, `ProductBrand="-1"`, `ProductGroup="-1"`, `Qc="-1"`, `RegisterDevice="0"`, `TechnicalCodeLike=false`. **Bug:** `ProductOldSerial` is sent as `ToTime` when the old-serial box is non-empty (should be the old serial).
- **Validation:** a search is only executed when the production line is selected; otherwise the box "لطفا ابتدا سالن تولید مورد نظر را انتخاب کنید" is shown and focus goes to the line combo. (The search only runs while `FlagChange` is true; the Search button sets it, grid Enter/Leave toggle it.)
- Three RPCs are called in sequence: `SRepRegisterTagSummaryForRegisterWinApp(filter)` (tab 1 data), `SRepRegisterTagSummaryGroupBySizeANDRegCode(filter)` (tab 2, a **dynamic-column** pivot returned as JSON and converted by `Tools.JsonToObjects`), `SRepRegisterTagDetailsForWinApp(filter)` (the detail rows).

**Tab 1 — "گزارش تولید - تجمعی سالن و کد کالا"** (`dgvSearchTajamoee`): columns سالن تولید (`fld_ProductPropertyATitle`), کد طرح, عنوان کالا, سایز کالا, کد کالا, رنگ کالا, سند درخواست (`ContractStatus`), تعداد (count of packs; the API returns it in the `ProductSerial` field), جمع مقدار (`ProductCount`). A final "همه" (total) row is appended (pack count, value sum). Summary box "خلاصه گزارش": تعداد کد کالا (= grid row count including the total row), تعداد پالت (sum of pack counts), جمع مقدار (sum of values).
**Tab 2 — "گزارش تولید - تجمعی سایز و کد طرح"** (`dataGridView1`, bound to a DataTable): fixed columns خط تولید, کد طرح (`RegCode`), سایز, عنوان طرح (`ProductType`) and **one column per quality status** (the sample mapping used for printing: `1`, `2`, `3`, `SPC`, `اختلاطی 6/7/8`, `دپو`). A total row `"همه"` is appended (the code supports between 1 and 9 dynamic columns — `switch` on count; more columns → no total row). Empty cells → "0".
**Detail grid** (`dgvSearchItems`, "گزارش ریز تولیدات ثبت شده (رجیستر شده)"): ردیف, سریال, کد کالا, عنوان کالا, سایز کالا, کد طرح, رنگ کالا, مقدار, تاریخ وساعت, کاربر, خط تولید, شیفت تولید, ورود به انبار (`ProductProperties`), وضعیت (`TagStatusTitle`), hidden history button (the history form is commented out). It is filled **by clicking** a row in tab 1 (rows whose `ProductCode` equals the clicked row's code; the total row → all) or in tab 2 (rows with the same line + reg-code + size).
**Print ("چاپ")**: FastReport `reports\RepRegisterTab2.frx`, data source "Table" from tab-2 rows mapped to `RepPrintType2` (Line, Size, RegCode, ProductType, ProductStatus1/2/3/SPC/Ekh6/7/8/Depo; null → "0"), parameters `RepTime`, `RepDate` (Shamsi now), `RepFilter` (a text built from the filled filters; if a date is given without a time " 06:00 " is added — **the business day starts at 06:00**), printer = `Settings.PrinterModel`, silent print. The column-name → property mapping is hard-coded Persian (`خط تولید`, `سایز`, `کد طرح`, `عنوان طرح`, `اختلاطی 6`, …, `دپو`) so unknown status columns are ignored.
**Excel**: save dialog (`*.xls;*.xlsx`) → `AvizheExcel.ExcelExport.ExportDatatable(DataTable-from-grid, path)`, boxes "فایل اکسل مورد نظر در مسیر انتخاب شده ذخیره شد" / "ذخیره فایل اکسل با خطا مواجه شده است".
**Gotchas:** the `ProductSerial` field is overloaded as a count in the summary DTO; the detail grid is only populated by clicks; `CountPack += Convert.ToInt32(pr.ProductSerial)` throws if the server returns a non-integer.

---

### F15. `frmRepCancelRegister` (`UI/StoreProduct/frmRepCancelRegister.cs`) — cancelled-registration report

Filters: سریال (`txtProductSerial`), از/تا تاریخ (masked; a value is only used when its length ≥ 10), کاربر ابطال (`cmbUsers` from `GetAllUser`). Buttons: جستجو / جدید / Excel (same export as F14). Load: fill users, clear.
Search (only after the Search button sets the flag): RPC `SRepSCancelRegisterTag(ProductSerial, FromDate, ToDate, User)` — **dates are sent with slashes as typed** (unlike F14); unselected filters are `""` (not `"-1"`). Grid: ردیف, سریال, کد کالا, کد تولید (`ProductRegCode`), عنوان کالا, مقدار کالا, کاربر ثبت (`RegisterUser`), تاریخ ثبت (`RegisterDate`), تاریخ ابطال (`SoftDeleteDate`), کاربر ابطال (`SoftDeleteUser`). Server column names: ProductSerial, ProductCode, ProductRegCode, ProductName, ProductCount, RegisterUser, RegisterDate, SoftDeleteDate, SoftDeleteUser.

### F16. `frmRepErrorInRegisterTag` (`UI/StoreProduct/frmRepErrorInRegisterTag.cs`) — data-quality report (two grids, no filters)

On load it runs two RPCs and shows two identical-column grids (ردیف, سریال, کد کالا, کد تولید, عنوان کالا, مقدار کالا, کاربر ثبت, تاریخ ثبت):
1. "کالاهای رجیستر شده که از بسته بندی خارج نشده اند" (registered goods that have not left packaging): RPC `SRepRegisterTagByTagStatus0(FromRegisterShamsiUnixDate = Time.GetUnixPersianTime(-1))` (Persian-calendar unix-style string for *yesterday*, i.e. the last day).
2. "کالاهای رجیستر شده که شناسه تگ ندارند" (registered goods without a tag id): RPC `SRepRegisterTagWithOutEPC` (no parameters).
Server column names: ProductSerial, ProductCode, RegCode, ProductName, ProductCount, Username, TagRegisterShamsiUnixDate. (The labels are swapped relative to the designer variable names: `lblPrintActionId` heads the first grid.)

---

### F17. `FormSetting` (`UI/formSetting.cs`) — settings editor

**Opened from:** the login form (gear button) and the hub. Modal. Window title = `"تنظیمات - " + Settings.PrintPaperType`. **Save** button / `Ctrl+S`. The save writes **every field both into `Properties.Settings` (user-scope, `Save()`) and, via `GetSettingDictionary.SaveSettingInFile()`, into the file-backed dictionary** (the dictionary itself is not modified here, so its values keep the pre-edit values; only values already present as keys are re-written). On success `DialogResult.OK`; on any exception the box "در انجام عملیات مشکلی بوجود آمده است" is shown. The window needs an **application restart or re-read** for most settings (they are read at hub construction time).

**Tabs and fields** (control → `Settings` key; stored text):
| Tab | Field (Persian) | Control | Key / encoding |
|---|---|---|---|
| تنظیمات نرم افزار | آی‌پی سرور | `txtServerIp` | `ServerIP` (`host:port`, e.g. `localhost:97`) |
| | نام شرکت | `txtCompanyName` | `CompanyName` |
| | تعداد کارکتر سریال | `txtSerialLength` | `SerialLength` int (empty → 5) |
| | نوع رجیستر (cmbKindRegister) | combo | `KindRegister` = **selected index as string** (0 "نا مشخص" undefined, 1 "رجیستر از طریق RFID Lable Printer", 2 "رجیستر از طریق بارفید", 3 "رجیستر با چاپ بارکد", 4 "رجیستر تک سریال با ریدر رو میزی") |
| | ورود در نرم افزار با اعتبار سنجی | `chbLogin` | `HaveLogin` bool |
| | نرم افزار نوتیف | `chbNotifApp` | `NotifApp` bool |
| | تشخیص خودکار شیفت کاری | `chbAutoDetecteShift` | `AutoDetecteShift` "1"/"0" |
| | دریافت لیست سریال | `chbGetListSerial` | `GetListSerial` "1"/"0" |
| | چاپ بصورت فایل PDF | `chbMergPrintToPDF` | `MergPrintToPDF` "1"/"0" |
| | تظبیق خودکار قالب چاپ با برند کالا | `chbControlPrintFormatByBrand` | `ControlPrintFormatByBrand` "1"/"0" |
| | امکان گروه بندی سریال ها | `chbSetParentForSerials` | `SetParentForSerials` bool |
| | مجوز مشاهده فرم مدیریت چاپ قبل از ثبت عملیات | `chbAllowShowOnlyPrintFormBeforeSavePrintAction` | bool |
| | تعداد رجیستر همیشه 1 باشد | `chbRegisterCountEvryTimeIsOne` | `RegisterCountEvryTimeIsOne` bool |
| | پیش نمایش اولین ردیف قبل از چاپ | `chbPrintPreviewBeforePrint` | bool |
| | فعال سازی عملیات رجیستر فقط با یک سریال | `chbActiveRegisterFormByOneSerial` | bool |
| | (hidden) وجود پرینت در عملیات ثبت / نمایش دیالوگ بعد از هر عملیات ثبت | `chbPrinterInOperation`, `rbnShowSaveDialog` | `PrinterInOperation`, `ShowSaveDialog` bool |
| تنظیمات چاپ | مدل چاپگر | `cmbInstalledPrinter` (installed Windows printers) | `PrinterModel` (name or "") |
| | ZPL | `chbZpl` | `IsZpl` bool |
| | (hidden) نوع چاپ | `cmbKindPrint` {A5, A6} | `PrintPaperType` |
| | (hidden) فایل گزارش برای چاپ | grid of `reports\*` files, single-select checkbox | `ReportFileName` |
| تنظیمات Rfid | مدل میز | `cmbDeviceModel` {6061, Tiny, T64} | `TableModel` = index string **and** `DeskReaderType` = index int |
| | پورت اتصال | `txtReaderPort` | `DeskUHFReader_Port` **and** `PortNumber` (same text) |
| | قدرت انتشار | `txtReaderPower` | `ReaderPower` int (empty → 10) |
| | ignoreEpcList | `txtIgnoreEpcList` | `IgnoreEpcList` |
| barcode (first tab) | سریال کالا / کد کالا / ID radio | `rbnBarCode*` | `BarcodeType` = `"Serial"` / `"Code"` / `"Id"` |
| | کاراکتر جداکننده میان داده ها | `txtDataSpliter` (default `,`, **required**: "جداکننده ی بارکد را مشخص کنید") | `BarcodeSpliter` |
| | ایندکس اصلی | `txtBarcodeIndex` (**required**, integer ≥ 0: messages "ایندکس را مشخص کنید", "لطفا در ورود ایندکس فقط از اعداد استفاده کنید", "ایندکس نمی تواند از صفر کوچکتر باشد") | `BarcodeIndex` int |
| | طول بار کد یا QRCode (… یا …) | `txtBarcodeLength1/2` | `BarcodeLength1/2` (strings; buggy parse: if empty falls into the TryParse branch and stores `0`) |
Notes: `ReaderPort`/`BarcodeSpliter` etc. are read elsewhere both from `Properties.Settings` and from `GetSettingDictionary.DicSetting[ConfigKeys.*]` (two stores); the 6061 multi-COM panel code is commented out; `FillModelComboBox` is a stub. The form's Load enables the model/printer lists only when `ServerIP` is non-empty.

---

### F18. `frmShowAlert` (`UI/frmShowAlert.cs`) — toast

Constructor `(string message)`; borderless small window anchored to the bottom-right of the working area, `TopMost`, plays the system "Hand" sound on load, **auto-closes after 30 s** (`timer1` 30000 ms) or on click of the label. Created by the notification poller (`SchasualedSmsCreator`, `new frmShowAlert(NotifMessage).Show()`; see support section).

---

## 8. Support code and hardware

### S1. Transport and error behaviour (`WorkingWithApiConnector`, `Connector`, `ApiBusiness`)
- `ApiBusiness.StringKey_Url` is built **once per `ApiBusiness` instance** from `Properties.Settings.Default.ServerIP`: `http://{ServerIP}/RfidCore/v1/Wms/PostObject`. Every call is `HTTP POST`, `Content-Type: application/json`, UTF-8 body `{"interface":"RestAPI","method":"<Rpc>","parameters":{…}}`, **60 s** timeout, no auth header/token, no retry (except where a form retries itself).
- Reply: `{"successful": bool, "value": …}`; callers test `result["successful"]` and then read `result["value"]` (a bool, a string, a JSON array, or — for pivot reports — a JSON **string** that is parsed again).
- **Failure behaviour:** `CreateSendDataDictionaryAsync` swallows every exception and returns `null`, so `ApiBusiness` methods then fail with a `NullReferenceException` (the login form specifically catches `WebException`/`AggregateException` for the "اتصال به سرور ممکن نیست" message). Most list methods return `null` when `successful` is false and the forms are written to survive that only partially.
- `RestAPI.cs`, `Connector.cs` (raw `HttpWebRequest` helpers) and the old `RestApi.Ip` property are legacy; `RestApi.Ip` is set at startup but not used by `ApiBusiness`.
- `Items` (static `DataAccess.Items`): `Login` (RPC `TSLogin`) treats a returned value of `"0"` as failure; otherwise `OnlineUserCode` = the returned value (the user id/token string), `OnlineUserName` = typed username, `OnlineUserRealName` = RPC `TSLoginReturnName(UserId)`. **Which of "code" or "name" each RPC expects differs** (see the RPC table): `userToken`/`userCode` parameters get `OnlineUserCode`; cancel/register/`username` parameters get `OnlineUserName`. `Items` also holds unused `NewCustomerCode`, `BranchCode`, `BranchName`.

### S2. Settings stores (two parallel mechanisms)
1. **`Properties.Settings`** (user-scoped `.NET` settings, `Settings.settings`/`app.config`): the typed store. Complete list with defaults is in the Settings table (appendix).
2. **`ApplicationConfig.log`** (file next to the exe, one line `key=value/key=value/…`): parsed at start into `GetSettingDictionary.DicSetting`; contains at least `ServerIP`, `ReaderPort`, `BarcodeSpliter` and the keys defined in `ConfigKeys`. `SaveSettingInFile()` rewrites the whole file from the dictionary and shows the box "تنظیمات با موفقیت ثبت شد" unless called from login. A missing/unreadable file → box "در ثبت فایل تنظیمات مشکلی بوجود آمده است" and the application **exits**.
`Program.cs` once copied typed settings into the dictionary, but those lines are **commented out**, so the dictionary contains only what the file already holds; `DicSetting[ConfigKeys.ReaderPort]` (read by F9/F10/F11/F12) throws `KeyNotFoundException` if the file lacks `ReaderPort`. Both stores are read by different code (e.g. `ReaderPort`/`BarcodeSpliter` from the dictionary, `DeskUHFReader_Port`/`SerialLength` from typed settings); `FormSetting` writes mostly the typed store. A rewrite should merge them into one configuration object and keep these key names for migrating existing installs.

### S3. Desk RFID reader abstraction (`BllConectToDeskReader`, `Classes/BLLConnectToUHFReader.cs`, 1954 lines)
- One facade class with the contract used by every form: `ConnectionStatus`, `StartIndentifyStatus`, `StartIndentifyTagByDeskReader(TextBox sink, DataGridView, int mode)`, `StopIndentifyTagByDeskReader()`, `ClearList()`.
- **Hardware selection** by `Settings.DeskReaderType` (written by the settings screen from the "مدل میز" combo): `0` = "6061" (vendor `RfidApi`: `OpenCommPort(DeskUHFReader_Port)`, `SetRf(power, freq=2)`, polling `EpcMultiTagIdentify`, 12-byte EPC → 24 hex chars), `1` = "Tiny" (USB-HID `UhfReader_API_HID.dll`: finds the first HID, `57600` baud / CRC flag, `UhfStartInventory`, a background thread reads inventory continuously), `2` = "T64" (`VHLib.dll` over USB: `VH_ConnectScannerUsb`, `EPC1G2_ReadLabelID` loop on a thread, up to 3 retries for version/basic/auto parameters). The factory default is `1`. Power = `Settings.ReaderPower` (default 7; `DeskPower` is used for T64).
- **Result delivery:** a WinForms timer (200 ms) polls the driver and writes the EPC into the form's (usually hidden) `TextBox`; forms then react to `TextChanged` when the text is exactly **24 hex chars**. Tiny/T64 strings longer than 24 are trimmed (Tiny: last 24; T64: first 24). Mode `1` = single read: the first new EPC is written and the reader is stopped automatically; mode `2` = continuous: every *new* EPC is written (the facade keeps a seen-list of EPCs already delivered; a 20-s `TimerClear` that would empty it is created but **never enabled**, so the list is only emptied by `Stop`/`ClearList()` — a tag already seen is not delivered again until the reader is restarted).
- **Ignore list:** T64 only — EPCs listed in `Settings.IgnoreEpcList` (comma-separated, trimmed) are dropped (used to skip fixed reference tags near the reader).
- **Lifecycle contract used by all forms:** on load stop any previous run, close the serial port by name (`new SerialPort(port).IsOpen` — a no-op in practice), start; on close or after a single result stop; failures are swallowed (a red status box is the only signal). Forms re-create the facade per form; the hardware is single-user, so only one form may read at a time (this is why opening a sub-dialog stops the reader first).
- **Equivalent in a rewrite:** an abstract "EPC source" emitting 24-hex strings (single-shot and streaming modes) + a configurable ignore list; the desktop vendor drivers (`RfidApiLib.dll`, `UhfReader_API_HID.dll`, `VHLib.dll`, `SLABHID*.dll`) are Windows-only native libraries.
- `ConnectTo6061.cs`, `GetComPort.cs` (COM-port enumeration for the commented-out multi-device panel) and `MR6100Api.dll` are unused by the current flows.

### S4. Barcode keyboard-wedge input
Scanners act as keyboards. Forms keep a text box focused via a timer, then process when the text length equals `Settings.SerialLength` (F11), on Enter / debounce timer (F8, F12). In F8 `DicSetting[BarcodeSpliter]` (single char) splits a compound barcode (3 parts → part 3, 2 parts → part 2, else whole). `BarcodeIndex`, `BarcodeType` ("Serial"/"Code"/"Id") and `BarcodeLength1/2` are **edited in the settings screen but never read by any form in the code read** (declared-only behaviour a rewrite may drop or implement fresh).

### S5. Notification poller (`SchasualedNotifCreator`) and tray (`TaskTrayApplicationContext`)
- Only active when `NotifApp` is true (login chooses the tray path instead of the main form).
- Tray: `NotifyIcon` "گزارشات RFID" (icon `logo_icon`), context menu: "گزارش نوتیف ها" (opens `frmRepRegisterTag`) and "خروج از حساب کابری" (`Application.Exit`).
- Poller: static `System.Windows.Forms.Timer` **30 s**; each tick RPC `SGetNotif_WinFormService(DateTimeLastCheck)` — if the returned string is non-empty it shows `frmShowAlert(message)` (30-s toast); then `LastCheck = now`. The large commented block (SMS summaries at 07:00/16:00, shift-end messages) is dead code; the SQL notes inside it are only a description of intended reports, do not implement them.
- `sms.cs` (an old SMS gateway client with embedded credentials) is not reachable from the live flows — **do not port it, and do not copy the credentials found there**; they should be treated as compromised and rotated.

### S6. Date and time helpers (`Time.cs`, `ShamsiCalendar.cs`)
- Shamsi (Persian/Jalali) calendar via .NET `PersianCalendar`. Display date: `yyyy/MM/dd`; compact timestamp ("unix Persian time"): `yyyyMMddHHmmss` (`GetUnixPersianTime(addDays)`), also stored on tags as `TagRegisterShamsiUnixDate`. Month/year template parameters: `ماه_تولید` = chars 4–5, `سال_تولید` = chars 2–3 of the Shamsi date string.
- `ConvertShortTimeToLongTime`, `GetDayNameFromDateTime`, `ExistMinBetweenToTime` are small utilities (minutes between two `HH:mm`). `ShamsiCalendar.cs` is a legacy converter (not used by the new flows).
- **Shift/business-day convention:** reports treat 06:00 as the day boundary (see F14 `RepFilter` text); `AutoDetecteShift` = "1" makes `frmStoreRegisterTags` pick the shift automatically by clock (see F2).

### S7. Excel, PDF and misc libraries
- `AvizheExcel.ExcelExport.ExportDatatable(DataTable, path)` — an in-house Excel writer (OpenXml); the app also references Syncfusion PDF, Stimulsoft (unused in the code read), PdfSharp (merge), FastReport 2017.1.22.
- `Tools.ConvertDatagridViewToDatatable` (grid → DataTable for export), `Tools.ExceptionLogger.WriteExceptionLogs` (daily file under `Exceptions\`), `Tools.JsonToObjects`, `Tools.ReplaceAllEnglishAlphbet` — all described in the appendix.
- `Program.cs`: `GetSettingDictionary.GetSetting()` → `RestApi.Ip = Settings.ServerIP` → `Application.Run(new FrmLogin())`; licence check and auto-start registry key are commented out.

---

## 9. Printing system (summary across forms)

| Mechanism | Forms | Selected by | Register at print? | Marks printed (`PrintUpdatePrintStatus`)? |
|---|---|---|---|---|
| Raw **ZPL** over the Windows spooler (`ZPLHelper`) | F6 | `KindRegister=1` and `IsZpl=true`; templates `zpl_Formats\*.txt` | yes (`SRegisterTag`, EPC ⚠ constant) | yes |
| **ZMIN** vendor SDK `LabelPrinter.dll` (`.lsf` templates in `zmin_Formats`) | F6 | `KindRegister=1` and `IsZpl=false` | yes (`SRegisterTag` with the **serial as EPC**) | yes |
| **FastReport** `.frx` templates in `Print_Formats` (file naming `Name_Printer_BrandCode.frx`) | F6 (always also available), F7 | any | F6: **no**; F7: only when `KindRegister=3` | **no** (neither form) |
| FastReport reports in `reports\` (`RepRegisterTab2.frx`), `PrintPackFormat.frx`, `PrintTestAfterRegister.frx` | F14, F11, F7 | — | — | — |

Common rules: template files are discovered by listing the folder at form load (one button per file); label text variables are substituted by **Persian token names** — ZPL/ZMIN as `[token]` text replacement, FastReport as **report parameters** (undeclared parameters are ignored); every key in the row's `ProductTechnicalData` and `ProductProperties` JSON is also exposed under its own title. The PDF-merge option exports each label to `tempfastreport\<n>.pdf` and combines them with PdfSharp into `MergPdf.pdf`. Token/parameter catalogs and the ZPL header conventions are in F6/F7. Printer selection: `Settings.PrinterModel`, or the printer name embedded in the template file name (token after the first `_`; `"0"` = use `PrinterModel`).

---

## 10. Business rules and state model

1. **Batch creation (F2):** exactly one `PrintActionId` per save; one print row per serial; serials come from (a) the server (count only), (b) a range generator, (c) an Excel column, with the "delete char" trick for prefixes. A product must match the selected document's items when a document is loaded; when `CheckRegisterByDocItems` is on the requested quantity must not exceed the document remainder.
2. **Print row life-cycle:** created (`PrintFlag=0`, `RegisterFlag=0`, `TagEpc=""`) → printed (`PrintFlag=1` via `PrintUpdatePrintStatus`) → registered (`RegisterFlag=1`, `TagEpc` set via `SRegisterTag`; in F6 ZPL/ZMIN these two happen in one step) → (optionally) grouped under a parent → cancelled (`SCancelRegisterTag`, soft delete). A row can be edited/deleted only while it has no EPC.
3. **Registration pairing (F8):** one EPC ↔ one serial; re-binding an EPC to another serial or re-registering a serial requires **explicit operator confirmation**; an error stops the pairing loop.
4. **Cancel (F9)** only if the tag's **current warehouse** has `allowCancel`. **Change date (F10)** only if not inspected and still in production warehouses. Both operate on the EPC read from the tag.
5. **Packaging (F11):** a package has its own tag; children are attached by serial list; detaching = parent `"-1"`.
6. **QA:** F12 compares label serial vs tag serial; F13 audits a stack of tags (green registered / red not registered, counts).
7. **Shifts/day:** the production day starts at **06:00** (report filter text, auto-shift: `SHIFT 1` 06–14, `SHIFT 2` 14–22, night unhandled ⚠).
8. **Access control:** only `HaveLogin` + a hardcoded list of usernames in the hub (not server-enforced); everything else relies on server behaviour.
9. **Dynamic fields:** server-defined extra attributes (`actionType 0`) per print row, rendered as text box/drop-down (F2, F6, F7 edit panels), saved as `ProductProperties`, exposed to templates by title. Must stay data-driven.
10. **Data typing:** dates/times/numbers are strings on the wire; Shamsi dates `yyyy/MM/dd` for display and `yyyyMMdd[HHmmss]` for filters/timestamps.
11. **Persistence of the operator's context:** none beyond settings; the hub re-fetches all lookups on every start.

---

## 11. Consolidated hardcoded values and defects (⚠)

**Hardcoded values:** F2 username whitelist (12 names — **not reproduced here; the new system must make it configurable/role-based**); default document id `"000"`; serial-range "delete char" defaults `0`/`4`; auto-shift labels `"SHIFT 1"`/`"SHIFT 2"` and the 06/14/22 boundaries; unit `"متر مربع"`; tile-size presets (`75*75` → 1.69/40/67.6, `60*60` → 1.44/72/103.68, `60*120` → 1.44/60/86.4); product-type parent id `"1"`; F6 ZPL EPC constant `000000000000000000004321`; ZMIN `readEpc="000"`; report column names `1/2/3/SPC/اختلاطی 6-8/دپو`; the three sample EPC-ignore tags; `Canon MF3010` as the auto-print printer; `DocumentType "1"` for the roll list; business day boundary 06:00; settings defaults in §3.3.

**Defects / traps a faithful rewrite must consciously decide on (none "fixed" in the legacy):**
- Server IP saved by F1 is not what `ApiBusiness` reads; two unsynchronized settings stores.
- F2: the `IsActive = true` *assignment* in the store filter; fire-and-forget save loop with no verification/rollback; night shift unhandled; header-JSON parsed by string splitting; hotkey F7 omits kind 3.
- F6: constant EPC in ZPL path; ZMIN registers the serial as EPC and stubs the read EPC; `[مقدار_محموله]` mapped to `ProductRegCode` in ZPL; retry blocks are dead code; FastReport path neither registers nor marks printed; `تاریخ تولید` parameter never populated.
- F7: preview computes the printer token wrongly; never calls `PrintUpdatePrintStatus`; `SGetRegisteredPrints` name vs client name "NotRegistered" semantics unclear.
- F8: "No" at the re-register prompt is treated as success; a failure stops the loop.
- F9: possible `NullReferenceException` when the tag's warehouse isn't in the list; permission flag not reset between reads.
- F10: invalid/incomplete date or time input is silently ignored; the reader is restarted on form closing.
- F11: removed serials stay in the duplicate-guard list; header clicks throw; delete result unchecked.
- F12: label-side status/title never shown (NullReference swallowed).
- F13: calls the API every second even without new tags.
- F14: `ProductOldSerial` filter sends `ToTime`; summary DTO overloads `ProductSerial` as a count; total row only for 1–9 dynamic columns.
- Transport: exceptions swallowed → `NullReferenceException`; user-culture `DateTime.ToString()` sent to the server; HTTP without TLS.
- `sms.cs` contains embedded credentials/phone lists (not ported; treat as compromised).

---

## 12. Corrections to the earlier overview and open questions

**Corrections to `Docs/Register-migration.md`:**
1. `frmOnlyPrint` is opened by `KindRegister ∈ {2,3,4}`, **not** by `MultiProductInOnePrintActionId`.
2. `frmRFIDRegisterByBarfid` is opened **only** from `frmOnlyPrint`; it is not opened from the hub or from `frmPrintRegsiterTags`. `frmPrintRegsiterTags` has shortcut buttons to cancel and group-test only.
3. `frmTestRegister` (single) is used when `KindRegister == 2`, otherwise `frmTestRegisterGroup`.
4. `FormSetting` is opened from the login form and from the hub's "تنظیمات" button.
5. The ZMIN path does **not** write the tag through the SDK (stubbed); it registers the serial as the EPC.
6. FastReport printing (F6/F7) does not mark rows printed; ZPL/ZMIN do.
7. `frmRepErrorInRegisterTag` has no filters; it asks for registrations from "yesterday" (`GetUnixPersianTime(-1)`).
8. `RestApi.Ip` is set at startup but `ApiBusiness` reads `Settings.ServerIP`.
9. The settings screen does not expose five hub/print flags (`MultiProductInOnePrintActionId`, `CheckRegisterByDocItems`, `CountInPackEqualCount`, `ComputeAverageProductPackValueFromAllWeight`, `AutoPrintAfterRegister`/`PrinterModelForAutoPrintAfterRegister`).
10. `AvizheExcel` reads `.xls` with the **Jet OLE DB** provider (Windows/32-bit-oriented); the import form (F2 Excel tab) takes the **first column** of each row.

**Open questions the client code cannot answer (need the server/stakeholders):**
- Real EPC generation scheme (only a commented formula exists in F6; `SCreateEpc` exists but is unused).
- Server semantics of `SGetRegisteredPrints` (client treats it as "not registered"), `TSLogin` return value (user id string; `"0"` = failure), `SIsProductSerialExist` status codes (0 not in print table, 1 queued, 2 registered).
- Whether `ProductType` is also sent by `SaveProduct` from F3 (the DTO field is filled from a combo; verify).
- Role/permission model replacing the hardcoded username list.
- Which deployments use which `KindRegister`/printer mechanism (all four paths exist).

---

## 13. Appendix

### 13.1 Helper functions
- `Tools.ReplaceAllEnglishAlphbet(s, "")`: upper-cases and removes `A–Z`, space, `:`, `-`, `/`, `,` (→ digits and non-ASCII remain). Used to derive the EPC from the serial when `KindRegister=3`.
- `Tools.JsonToObjects(JToken)`: converts a JSON array to a string `DataTable` (columns taken from the **first** object's property order, rows filled positionally); a value that serializes as `{}` → `"0"`; renames columns: `fld_ProductPropertyATitle`→`خط تولید`, `RegCode`→`کد طرح`, `ProductName`→`عنوان کالا`, `fld_ProductPropertyCTitle`→`سایز`, `ProductType`→`عنوان طرح` (these Persian names are then consumed by F14's report mapping).
- `Tools.ExceptionLogger.WriteExceptionLogs` (appends to `Exceptions\Exception<shamsi yyyy-MM-dd>.log`), `Tools.WriteLogs(method, text)` (`Log\Log<date>.log`), `Tools.ConvertDatagridViewToDatatable` (column headers become DataTable columns), `AvizheExcel.ExcelExport.ExportDatatable`, `AvizheExcel.ExcelReader.ReadExcelDataExportInDataTable` (Jet OLE DB, `Excel 8.0`), `Time.*` (see S6).
- `Print` creation in F2 sets `ProductContractType="0"`.

### 13.2 Shipped runtime assets (`bin/Debug`)
`Print_Formats\*.frx` (7 FastReport label templates; names like `لیبل دوم Camera_0_3.frx`, `…_Microsoft Print to PDF.frx`), `reports\*.frx` (e.g. `RepRegisterTab2.frx`), `PrintPackFormat.frx`, `PrintTestAfterRegister.frx`, `zpl_Formats\*.txt` (ZPL with `[token]` placeholders), `zmin_Formats\sample.lsf`, `Sample.xlsx` (import sample), `tempfastreport\` (PDF merge scratch), vendor DLLs (`LabelPrinter.dll`, `RfidApiLib.dll`, `UhfReader_API_HID.dll`, `SLABHID*.dll`, `VHLib.dll`, `MR6100Api.dll`), FastReport 2017.1.22, PdfSharp, Syncfusion.Pdf, Stimulsoft (referenced, unused in the code read), `AvizheExcel.dll`, `AvizheRestConnector.dll`.

### 13.3 Dependencies (packages.config)
Microsoft.Office.Interop.Excel 15.0.4795.1000, Newtonsoft.Json 13.0.1, PDFsharp 1.50.5147, Syncfusion.Compression.Base / Licensing / Pdf.WinForms 20.4.0.44; non-NuGet: FastReport, LabelPrinter (ZMIN SDK), RfidApiLib and the other vendor DLLs above.

### 13.4 Licensing (disabled)
`RegisteryClass` computes a node-locked activation code from the CPU id (WMI `Win32_Processor.ProcessorId`) plus a hardcoded seed and compares it to a registry value under `HKCU\SOFTWARE\TDAvizhe\RFIDConnect_antenna\RFIDConnectOfVars`; the check and the auto-start registry key are **commented out** in `Program.cs`.

### 13.5 Local database (unused)
`DataAccess` is a generic ADO.NET (`SqlClient`) executor (non-query, transaction, adapter, reader) over `Settings.ConnectionString`; no screen read uses it. `Items` holds the session globals.