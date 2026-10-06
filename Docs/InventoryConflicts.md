# Inventory Conflicts (مغایرت انبارگردانی) — Developer & AI-Agent Guide

> **Repository:** `Avizhegroup/silo-sync` — **branch:** `dev` (only this branch was reviewed)
> **Reviewed at commit:** `5f75f01` ("Add project files", 2026-08-23) — the only commit in the available history touching these files.
> **Scope:** the "Inventory Conflicts" report page and the six API methods it depends on (plus one extra dependency, `SSearchLastCAD`).
> Line numbers below refer to commit `5f75f01` and will drift; search by symbol name.

---

## 1. What the feature does

After a physical inventory count (انبارگردانی) done with RFID readers, the system holds two views of the same warehouse:

| View | Source | Meaning |
|---|---|---|
| **System / "Warehouse" list (A)** | `tbl_Tags` | Tags the system believes are in the selected warehouse(s) |
| **Inventory list (B)** | `tbl_InventoryTags` | Tags actually read during the inventory operation (`InventoryHeaderId`) |

The page compares A and B **per tag (serial)** and rolls the result up **per product code**. It additionally compares system quantities with two numbers a customer can upload by Excel:

| Extra view | Source | Meaning |
|---|---|---|
| **Accounting (حسابداری)** | `tbl_CustomerAccountingData.fld_CADProductCount` | Quantity from the customer's accounting system |
| **Physical count (شمارش فیزیکی / Reality)** | `tbl_CustomerAccountingData.fld_CADRealityCount` | Quantity from a manual count |

The user can then select conflicting tags and **"fix" them**, which registers a warehouse-entry movement for the selected EPCs.

### Glossary (Persian status strings are used as logic keys in code!)

| String | Meaning | Where produced |
|---|---|---|
| `مغایرت کسری` | **Shortage** — tag is in system list A but NOT read in inventory B | `CheckConflicts` |
| `مغایرت اضافی` | **Extra** — tag was read in inventory B but is NOT in system list A | `CheckConflicts` |
| `مغایرت کالایی حسابداری` | Product exists in accounting data but has no tags/inventory at all | `CheckConflicts` |
| `مغایرت مقداری حسابداری` | Quantity differs from accounting | `CheckConflicts` |
| `مغایرت کالایی شمارش فیزیکی` | Product-level conflict with physical count | `CheckConflicts` |
| `اضافی` / `کسری` / `مغایرت` | Substrings the UI matches with `Contains(...)` to filter/colour rows | UI |

> ⚠️ These Persian substrings are matched with `string.Contains`. Do **not** reword, translate, or "clean up" them without updating every `Contains` call in `InventoryConflicts.razor` and `.razor.cs`.

---

## 2. File map

| Layer | File | Role |
|---|---|---|
| UI markup | `Silo/Pages/Reports/InventoryConflicts.razor` | Route `/report/inventoryconflicts`; filters, main grid, summary bar, chart, 4 modals |
| UI code-behind | `Silo/Pages/Reports/InventoryConflicts.razor.cs` | Orchestration, conflict calculation, summaries, fix flow, Excel upload, PDF export |
| API business | `Silo.Api/Business/WmsBusiness.cs` | All 6 API methods (+ `SSearchLastCAD`) |
| Query/command models | `Silo.Application/Features/Inventory/**` | `GetInventoryConflictsQuery`, `SaveFixedConflictsCommand`, VMs, `NonDocFileTypeEnum` |
| Shared request model | `Silo.Application/Dto/IndexSearch.cs` → `InventoryRequest` | Model type the API methods `SGetProductOfWarehouse` / `SSearchInventoryTags` declare |
| Entities | `Silo.Domains/Entities/CustomerAccountingData.cs`, `NonDocFileLog.cs` | EF entities for uploads |

Key model classes:

- `GetInventoryConflictsQuery` — filter object sent as `search`.
- `GetInventoryConflictsVm` — one **row per product code** in the main grid (`Details` list = per-tag rows, `[JsonIgnore]`).
- `GetInventoryConflictDetailsVm` — one **row per tag** (serial/EPC) inside a product, carries `Status` and `IsSelected`.
- `GetWarehouseProductsVm` — a tag from the system side (also reused for accounting rows).
- `GetInventoryResultTagVm` — a tag from the inventory side.
- `GetWarehouseProductsListsVm` / `GetInventoryProductsListsVm` — `{ Mains, Exits, Enters }` triples for movement-filtered mode.
- `GetEnterAndExitProductValueVm` — `EnterCount/EnterSumCount/ExitCount/ExitSumCount` shown in the "Enter/Exit" tab.
- `SaveFixedConflictsCommand` — `{ WarehouseCode, Desc, Serials[] }`.
- `SaveNonDocFileCommand` — `{ FileName, Type, Data(JSON string) }`; `NonDocFileTypeEnum { CustomerAccountingData=0, ProductData=1, CustomerRealityCountData=2 }`.

---

## 3. End-to-end flow

```
User fills filters ──► OnValidSubmit
   │  FixEmptiness(): every empty filter becomes the string "-1"
   │
   ├─ IsMovementFiltered == false  (normal mode)
   │     SGetProductOfWarehouse     ─► TagsInWarehouse (A)
   │     SSearchInventoryTags       ─► TagsReadedInInventory (B)
   │
   └─ IsMovementFiltered == true   (movement-filtered / "freezed" mode)
         SGetFreezedTagProductsBeforeMovements       ─► {Mains,Exits,Enters} for A
         SGetFreezedInventoryProductsBeforeMovements ─► {Mains,Exits,Enters} for B
         CalculateFreezedLists():  A = Mains + Exits − Enters   (by ProductSerial)
                                   B = Mains + Exits − Enters   (by ProductSerial)
                                   + fills EnterExitValue summary

   SSearchLastCAD ─► Accounting (both modes)
   CheckConflicts()      → builds Result (list of GetInventoryConflictsVm)
   CalculateSummaries()  → totals + chart values
```

Then optional follow-ups: open per-product shortage/extra modal → select tags → **SFixInventoryConflicts**; upload Excel → **SSaveNdfLog**; export Excel/PDF.

### The `-1` convention
The client never sends `null`. `FixEmptiness()` replaces every empty string filter with `"-1"`, and the server tests `search.X.NotEquals("-1")` / `!search.X.Equals("-1")`. A **new filter field must follow this convention on both sides**, otherwise the server may throw a `NullReferenceException` on `.Equals`.

`GetInventoryConflictsQuery` marks `FromDate`, `ToDate`, `InventoryHeaderId`, `Warehouse` with `[Required]`, and the page uses `DataAnnotationsValidator`, so those four are validated on submit. (The `-1` handling for them in `FixEmptiness` is therefore defensive.)

### Warehouse multi-select
`Request.Warehouse` is a **comma-separated list** of destination codes built by `OnModalCheckboxChange`. Server methods split on `,` and build `IN (N'a',N'b')`.

---

## 4. The conflict algorithm (`CheckConflicts` in `.razor.cs`)

Inputs: `TagsInWarehouse` (A), `TagsReadedInInventory` (B), `Accounting`.
Output: `Result` — product-level rows ordered by `ProductCode`.

**Step 1 — iterate A (system tags).** Get-or-create the product row. For each tag:
- `Count++`, `SumCount += ProductCount`
- if the serial exists in B → `CountInventory++`, `SumCountInventory += inventory ProductCount`
- else → **shortage**: `ConflictCount++`, `ConflictSumCount -= ProductCount`, detail `Status = "مغایرت کسری"`
- add a detail row (status empty when matched).

**Step 2 — iterate B (inventory tags) not found in A.** Get-or-create product row, then **extra**: `ConflictCount++`, `CountInventory++`, `ConflictSumCount += ProductCount`, `SumCountInventory += ProductCount`, detail `Status = "مغایرت اضافی"`.

**Step 3 — accounting rows** (`SSearchLastCAD`), per product code:
- product not in `Result` → new row with `SumCountAccounting = ConflictSumCountAccounting = accounting count`, `ConflictSumCountReality = RealityCount`, one detail `مغایرت کالایی حسابداری`.
- product exists but has **no inventory tags** → subtract accounting/reality counts from the conflict fields and append `- مغایرت کالایی حسابداری` / `- مغایرت کالایی شمارش فیزیکی` to **every** detail status.
- product exists with inventory tags → `diff = Σ inventory count − accounting count` (and same for reality); non-zero diff overwrites `ConflictSumCountAccounting` / `ConflictSumCountReality` and appends `- مغایرت مقداری حسابداری` to every detail.

**Sign convention:** shortage conflicts are **negative** in `ConflictSumCount`, extras **positive**.

### Summary bar & chart (`CalculateSummaries`)
Sums of the `Result` columns: product-code count, RFID count/sum, accounting sum, reality sum, RFID conflict count/sum, accounting conflict sum, reality conflict sum, inventory count/sum. The bar chart shows four values: RFID, accounting, physical count, inventory.

### Movement-filtered ("freezed") mode
Purpose: reconstruct what the lists looked like despite tags that moved in/out of the warehouse during the date range.
- **Mains** — tags currently matching the filters (A: current `tbl_Tags` rows in the warehouse; B: current tags whose EPC appears in `tbl_InventoryTags`).
- **Exits** — serials with a `tbl_TagsMovement` row leaving the warehouse inside `[FromDate, ToDate]`, minus serials that are still present (A) / appear in the inventory (B).
- **Enters** — serials with a movement *into* the warehouse inside the range.
- Final list = `Mains ∪ Exits` minus serials in `Enters` (`ExceptBy ProductSerial`).
- `EnterExitValue` counts exits/enters (see issue #9 — the counting branches are currently identical).

---

## 5. API reference

All are called through `RfidConnectApi.PostAsync` / `PostAsyncByContext` by **method name** with named parameters (`search`, `command`, `commands`).

### 5.1 `SGetProductOfWarehouse(InventoryRequest search) → DataTable`
- **Used when:** normal mode, system side (A).
- **Filters honoured:** `ProductCode`, `TechnicalCode` (exact, **ignores** `TechnicalCodeLike`), `Warehouse` (IN list on `tbl_Tags.TagInDestinationId`), `Type`, `Qc`, `Size`.
- **Filters ignored:** dates, user, desc, place, `InventoryHeaderId`.
- **Tables:** `tbl_Products RIGHT JOIN tbl_Tags`, subselects on `tbl_Tags` (zones) and `tbl_Destination`.
- **Returns columns:** `ProductCode, ProductTitle, RegCode, Qc, ProductSize, ProductSerial, ProductCount, Epc, Zones, ThisTagZone, Date, ContractStatus, DestinationTitle, Place` → deserialised to `GetWarehouseProductsVm`.
- Note: the main query has no `TagStatus = 1` filter (only the `Zones` subselect uses it).

### 5.2 `SSearchInventoryTags(InventoryRequest search) → DataTable`
- **Used when:** normal mode, inventory side (B).
- **Filters:** `InventoryHeaderId` (unquoted numeric), `FromDate`/`ToDate` (Persian → Gregorian + shift-start time), `Desc` (LIKE), `Place`, `Warehouse` (IN on `fld_InventoryStoreCode`), `User`, plus tag-level filters `ProductCode/Type/Size/Qc/TechnicalCode(+Like)`.
- **Tables:** `tbl_InventoryTags LEFT JOIN tbl_Tags ON TagEpc = fld_InventoryTagEPC`; calls `SPGetShiftStartENDList()` for the first shift's start time.
- **Returns:** `InventoryDate, InventoryHeaderId, InventoryZone, Epc, ProductSerial, ProductName, ProductCode, ProductCount, RegCode, RegisterDate, Date, ContractStatus, DestinationTitle, Place` → `GetInventoryResultTagVm`. `SELECT DISTINCT`, 300 s timeout.
- Both date bounds use **the same time-of-day** (first shift start, `HH:MM:01`), so `ToDate` effectively ends at the start of that day.

### 5.3 `SSearchLastCAD(GetInventoryConflictsQuery search) → DataTable` *(extra dependency, not in your list)*
Reads `tbl_CustomerAccountingData INNER JOIN tbl_Products`. Filters: `InventoryHeaderId` (`fld_CADInventoryHeaderId`), `ProductCode`, `Type`, `Size`, `Qc`, `TechnicalCode(+Like)`. Returns `ProductCode, ProductTitle, Qc, ProductSize, Regcode, ProductCount (accounting), RealityCount`.

### 5.4 `SGetFreezedTagProductsBeforeMovements(GetInventoryConflictsQuery) → GetWarehouseProductsListsVm`
Runs three queries (`mainCommand`, `exitedCommand`, `enteredCommand`) against `tbl_Tags`, `tbl_TagsMovement`, `tbl_MovementActions`; `TagStatus` marker 0/1/2 = main/exit/enter. Date filters become `RTagsMovementDateTime` bounds; warehouse filter sets store/destination conditions (exit: from store IN list **and** destination NOT IN list; enter: reverse + tag currently in list).

### 5.5 `SGetFreezedInventoryProductsBeforeMovements(GetInventoryConflictsQuery) → GetInventoryProductsListsVm`
Same triple for the inventory side, additionally filtered through `tbl_InventoryTags` aliases (`InvTagsMain`, `InvTags1`, `InvTags2`) by header id, date, store, desc, place, user.

### 5.6 `SSaveNdfLog(List<SaveNonDocFileCommand> commands) → bool`
Generic "non-document file" ingest. Steps:
1. `SaveLogs()` — inserts every command into `NonDocFileLog` (type taken from the **first** command).
2. By `Type`: `CustomerAccountingData` → `SaveAccounting`, `CustomerRealityCountData` → `SaveRealityCountData`, `ProductData` → `SaveProduct` (batch product import, also used by `SCheckCustomerProducts`).
- Accounting/reality upserts key on `(InventoryHeaderId, ProductCode)` in `CustomerAccountingDatas`; existing row → update `ProductCount` (accounting) or `RealityCount` (reality); else insert with a new `OpCode = max(OpCode)+1` (first row: `1`).
- Payload JSON is `GetInventoryConflictsExcelVm` with **camelCase** names (`productCode`, `sumCount`, `inventoryHeaderId`, `realityCount`) parsed with `JToken`.

### 5.7 `SFixInventoryConflicts(SaveFixedConflictsCommand) → bool`
1. `SCreateNewUHFReaderLogHeader(StationCode="0")` → new log id.
2. `SSaveGateLog(user, logId, "WEB APP", EPC list, null, "0")` → inserts `tbl_UHF_ReaderLog` rows for the selected EPCs.
3. `SSaveMovementAction(logId, "", WarehouseCode, "WEB APP", user, "0", "", "0", desc)` — creates a movement; description text is `ثبت ورود کالا به انبار : <WarehouseCode> رفع مغایرت - توضیحات: <Desc>`. Source location is derived from the first tag's current `TagInDestinationId`.
- It always performs a **warehouse-entry** movement into `WarehouseCode`, for both shortage and extra selections.

---

## 6. UI reference

### Filter tabs
1. **Operation info:** operation code (`InventoryHeaderId`), user, from/to date (Persian date pickers), placement (`Place`), description, warehouse multi-select modal.
2. **Product info:** product code (+ picker modal), technical code (+ LIKE checkbox), "show conflicts" (adds table border class), size, type, QC.
3. **Enter/Exit:** toggle `IsMovementFiltered`; four read-only numeric boxes show enter/exit counts and sums.

Buttons: upload reality-count Excel, upload accounting Excel (both disabled with a validation toast until an operation code is entered), clear, submit.

### Main grid (product level)
Columns: code, name, technical code, zones, RFID count/sum, accounting, reality, inventory count/sum, RFID conflict count/sum, accounting conflict, reality conflict, plus two buttons that open the **shortage** / **extra** modals showing `Details.Count(Status contains "کسری"/"اضافی")`. Row CSS: `rfid-conf` when RFID conflicts ≠ 0, `acc-conf` when only accounting conflict ≠ 0. Toolbar: Excel export, PDF (`PreparedReport/Create`, report `Inventory`), "all details".

### Fix-conflicts modal
Pre-filtered to the chosen status, local search (serial, contract status, zone, warehouse, register-date range), checkbox selection (**all selected tags must share one status**), destination warehouse (required) + description, save → `SFixInventoryConflicts`. After success, `RecalculateFixedConflicts()` adjusts the in-memory row (no re-query).

### Excel upload
`.xlsx`, max 5 MB (`MaxAllowedSizeMB`). Read via `DataTableTools.ReadExcelDataOutDataTable`; column 0 = product code, column 1 = number (rounded to 2 decimals; non-numeric → 0). One `SaveNonDocFileCommand` per row.

---

## 7. Database objects touched

`tbl_Tags`, `tbl_Products`, `tbl_Destination`, `tbl_InventoryTags`, `tbl_TagsMovement`, `tbl_MovementActions`, `tbl_ProductStatus`, `tbl_ProductPropertyC`, `tbl_CustomerAccountingData`, non-doc-file log table (`NonDocFileLog`), `tbl_UHF_ReaderLog`, UHF log header table, shift list via `SPGetShiftStartENDList()`.

---

## 8. Known issues & risks (verified by reading the code)

Severity: 🔴 breaks behaviour / security · 🟠 wrong results in some cases · 🟡 minor/cosmetic.

| # | Sev | Location | Issue |
|---|---|---|---|
| 1 | 🔴 | `SGetFreezedTagProductsBeforeMovements` — `exitedCommand` and `enteredCommand` | `{productFilters}` is appended **without a leading `AND`** (`... ) Tags.ProductCode = N'x'`). Any product-level filter (code, technical code, type, QC, size) in movement-filtered mode yields invalid SQL. The inventory counterpart correctly uses `" AND " + ...`. |
| 2 | 🔴 | All six API methods | Filters are built by **string concatenation** from request values → SQL injection risk. Use parameters (`dataAccess` already supports `KeyValuePair<string,object>[]` elsewhere). |
| 3 | 🟠 | `SGetFreezedInventoryProductsBeforeMovements`, Place filter | Uses `search.Desc` instead of `search.Place` in the three `fld_InventoryPlace = ...` conditions. |
| 4 | 🟠 | `OnToggleSelectAll` | Always `Serials.Add(p)` even when unchecking; "select all" repeatedly adds duplicates and "unselect all" never removes. Also adds even if the single-status validation failed for some rows. |
| 5 | 🟠 | `CheckConflicts` | `AsParallel().ForAll` mutates shared `GetInventoryConflictsVm` objects (`Count++`, `+=`, `Details.Add`, `LocationList.Add`) without locking → race conditions / lost updates. `ConcurrentDictionary` protects only the dictionary. Also `TagsInWarehouse.Any(...)` inside the B loop is O(n·m). |
| 6 | 🟠 | `OnClickDetails` / `OnClickShowAllDetails` | `ProductForDetails` is never assigned (stays `new()`), so the "all details" modal is empty. Also `IsDetailsShown` is forced to `false` and then tested, so the branch is dead; `IsAllDetailsShown` toggles but isn't what the branch reads. |
| 7 | 🟠 | `CheckConflicts` accounting block | The reality-count status appended uses the **accounting** text (`مغایرت مقداری حسابداری`) — copy/paste; the first branch appends both accounting+reality texts to every detail, including already-matched tags. |
| 8 | 🟠 | `RecalculateFixedConflicts` | `SumCountReality - SumCountReality` is always 0 (so `ConflictSumCountReality` becomes 0); `Count` is decremented once per selected serial regardless of conflict type. In-memory only — a re-query will show real numbers. |
| 9 | 🟡 | `CalculateFreezedLists` | In both exit and enter loops the `if` and `else` branches do the same thing, so the "matched" distinction has no effect. |
| 10 | 🟠 | Freezed-mode VMs | The row mapping in `SGetFreezed*` does not read `ContractStatus`, `DestinationTitle`, `Place`, `Date` (tag side) or `RegCode`, `RegisterDate` (inventory side) even though the SQL selects some of them, so those columns are empty in detail grids / exports in movement-filtered mode. |
| 11 | 🟠 | `SSaveNdfLog` | (a) Duplicate product codes in one Excel create duplicate rows (existence check hits only the DB, not the pending list). (b) `SaveChanges() > 0` returns `false` when a re-upload changes nothing, so the UI shows a failure although logs were saved. (c) Mixed command types are not handled (type = first command). (d) Empty file → `false`. |
| 12 | 🟠 | `SFixInventoryConflicts` | No null/empty-`Serials` validation; header creation, gate log, and movement are separate operations (no overall transaction), so a failure leaves an orphan UHF log header/rows. Same entry movement is used for shortage and extra. |
| 13 | 🟡 | `OnDetailsRowRenderHandler` | Checks `Contains("اضافه")` but the status text is `اضافی`; the second CSS branch never fires. Also `classStr` concatenation lacks a separator. |
| 14 | 🟡 | `SSearchInventoryTags` & freezed queries | `ToDate` uses the first shift's start time (not end of day), so records later on `ToDate` are excluded. Confirm this is intended. |
| 15 | 🟡 | `OnClickExportToPdf*` | On API failure the method returns without resetting `IsLoading = false` (spinner stays). |
| 16 | 🟡 | Method signatures | `SGetProductOfWarehouse` / `SSearchInventoryTags` take `InventoryRequest`, while the UI posts a `GetInventoryConflictsQuery`. It works only because property names overlap; `InventoryRequest` lacks `IsMovementFiltered`. |

---

## 9. Rules for AI coding agents working here

1. **Branch:** work only on `dev`; do not touch other branches.
2. **Keep the three layers in sync:** if you change an API method's output columns, update the matching VM (`GetWarehouseProductsVm`, `GetInventoryResultTagVm`), the JSON source-generator contexts (`GetWarehouseProductsVmContext`, `GetInventoryResultTagVmContext`, `GetInventoryConflictResponseContext`) and the UI.
3. **Status strings are logic.** Do not change the Persian substrings (§1) without updating all `Contains` checks, the grid buttons' counts, and the PDF report templates (`Inventory`, `InventoryDetails`).
4. **Use the `-1` convention** for new optional filters (client `FixEmptiness` + server `NotEquals("-1")`).
5. **Never add more string-concatenated SQL.** New or modified queries must be parameterised; fix existing ones when you touch them (issue #2).
6. **Normal and movement-filtered mode must give the same shape** to `CheckConflicts` (lists A and B of the same VM types). Test both paths.
7. **Preserve sign convention** (shortage negative, extra positive) — summaries and PDF depend on it.
8. **Excel contract:** column 0 = product code, column 1 = numeric; JSON property names are camelCase (`GetInventoryConflictsExcelVm`).
9. **Verify** with at least: normal mode with one warehouse; multiple warehouses; movement-filtered mode with a product filter (issue #1 repro); shortage fix; extra fix; both Excel uploads twice in a row (issue #11b).
10. Start fixing in this order of value/risk: #1, #3, #4, #11, #5, #6, then the rest, then #2 as a dedicated refactor.

---

## 10. Open questions for the domain owner

- In movement-filtered mode, list **B** adds *exited-but-not-scanned* tags (`Exits`) to the inventory list. Is that intended, or should exits only be added to list **A**?
- Should "fix" for a **shortage** really register a warehouse *entry*? (The tag is already in the system for that warehouse.)
- Should `ToDate` include the whole day?
- Should accounting/reality uploads be per `InventoryHeaderId` only, or also per warehouse?

---

## 11. Change log of this document

- **2026-10-05** — Initial version, written from a read-only review of `dev` @ `5f75f01`. History for the reviewed files contains only the initial import commit, so there were no earlier functional changes to merge in. **No source code was modified**; items in §8 are documented findings, not applied fixes.
