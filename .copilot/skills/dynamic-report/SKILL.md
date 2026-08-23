---
name: dynamic-report
description: "Scaffold a full dynamic report page in the Silo WMS project using the ReportAllSection component — including filter/column enums, .razor page, .razor.cs code-behind, event handlers, backend ReportBusiness method, and Excel export."
---

# /dynamic-report

Create a complete dynamic report page in the Silo project using the `ReportAllSection` component.

## Usage

```
/dynamic-report <ReportName>    # scaffold all files for a new dynamic report
/dynamic-report --checklist     # print the step-by-step checklist
```

If invoked with a description (e.g. `/dynamic-report inventory movement report`), infer the report name and domain, then scaffold all files.

---

## What You Must Do When Invoked

Execute all 8 steps below in order. Generate every file. Replace all `YourReport` placeholders with the actual report name.

---

## Files to Create

| File | Location |
|------|----------|
| `{Name}FilterType.cs` | `Silo.Application\Features\{Domain}\Enums\Dynamic\` |
| `{Name}ColumnsType.cs` | `Silo.Application\Features\{Domain}\Enums\Dynamic\` |
| `{Name}Dynamic.razor` | `Silo\Pages\DynamicReports\` |
| `{Name}Dynamic.razor.cs` | `Silo\Pages\DynamicReports\` |
| Backend method | `Silo.Api\Business\ReportBusiness.cs` |

---

## Step 1: Create Enum Types

### Filter Type Enum
**`Silo.Application\Features\{Domain}\Enums\Dynamic\{Name}FilterType.cs`**

```csharp
namespace Silo.Application.Features;

public enum {Name}FilterType
{
    FromDate,
    ToDate,
    ProductCode,
    Size,
    ProductGroup,
    ProductBrand,
    ProductType,
    // Add report-specific filters here
}
```

### Column Type Enum
**`Silo.Application\Features\{Domain}\Enums\Dynamic\{Name}ColumnsType.cs`**

```csharp
namespace Silo.Application.Features;

public enum {Name}ColumnsType
{
    OperationCode,
    PersianDateFull,
    PersianDateYear,
    PersianDateMonth,
    PersianDateDay,
    ProductSerial,
    ProductCode,
    ProductCount,
    ProductName,
    DynamicFields,           // for dynamic field columns
    DataMiningElements,      // for data mining element columns
    // Add report-specific columns here
}
```

---

## Step 2: Create the Razor Page

**`Silo\Pages\DynamicReports\{Name}Dynamic.razor`**

```razor
@page "/dynamicreport/{name-kebab}"
@page "/dynamicreport/{name-kebab}/{FormatId:int?}"
@inherits SiloBasePage
@namespace Silo.Pages.DynamicReports
@using Silo.Application.Dto.Filter
@using Silo.Application.Features
@using Silo.Shared.Components.Report
@using System.Text.Json
@using Telerik.FontIcons

<PageTitle>@PageTitle</PageTitle>

@if (IsFiltersShown)
{
    @if (IsInitPageFinished)
    {
        <CascadingValue Value="Api">
            <CascadingValue Value="Notification">
                <CascadingValue Value="IsLoading">
                    <ReportAllSection @ref="ReportAllSectionRef"
                        TFilter="{Name}FilterType"
                        TColumn="{Name}ColumnsType"
                        Filters="Filters.Where(p => p.IsFilterShown).ToList()"
                        DataColumns="DataColumns.Where(p => p.IsColumnShown).ToList()"
                        DataMiningElementColumns="DataMiningElementColumns"
                        CalculatingColumns="CalculatingColumns"
                        PivotColumns="PivotColumns"
                        AddedFilters="ApplyFilters"
                        AddedDataColumns="AddedDataColumns"
                        AddedDataMiningElementColumns="AddedDataMiningElementColumns"
                        AddedCalculatingColumns="AddedCalculatingColumns"
                        AddedPivotColumn="AddedPivotColumn"
                        OnDataColumnAddClick="OnDataColumnAdd"
                        OnDataMiningElementColumnAddClick="OnDataMiningElementColumnAdd"
                        OnCalculatingColumnAddClick="OnCalculatingColumnAdd"
                        OnPivotColumnAddClick="OnPivotColumnAdd"
                        OnDataColumnRemoveClick="OnDataColumnRemove"
                        OnDataMiningElementColumnRemoveClick="OnDataMiningElementColumnRemove"
                        OnCalculatingColumnRemoveClick="OnCalculatingColumnRemove"
                        OnPivotColumnRemoveClick="OnPivotColumnRemove"
                        OnSearchClick="OnSearchClick"
                        OnAddNewFilterClick="OnAddNewFilter"
                        OnFilterRemoveClick="OnFilterRemoveClick"
                        IsColumnsEditable="FormatId is null">
                    </ReportAllSection>
                </CascadingValue>
            </CascadingValue>
        </CascadingValue>
    }
}

@if (Results.Any() && !IsLoading)
{
    <TelerikGrid Data="Results"
                 PageSize="10"
                 Navigable="true"
                 Pageable="true"
                 Sortable="true"
                 ScrollMode="GridScrollMode.Scrollable"
                 Resizable="true"
                 Width="100%">
        <RowTemplate Context="data">
            @{
                JsonElement row = (JsonElement)data;
                decimal rowCount = 0;
            }
            @foreach (var item in GridColumns)
            {
                @if (row.TryGetProperty(item, out JsonElement element))
                {
                    string valueString = element.ToString().Replace("{}", "0");
                    if (valueString.HasNoValue()) valueString = "0";
                    <td class="td-style">@valueString</td>
                    @if (ColumnAgg is not null && ColumnAgg.ContainsKey(item))
                    {
                        if (decimal.TryParse(valueString, out decimal d)) rowCount += d;
                        else rowCount++;
                    }
                }
            }
            @if (AddedPivotColumn is not null)
            {
                <td class="td-style">@rowCount</td>
            }
        </RowTemplate>
        <GridColumns>
            @foreach (var item in GridColumns)
            {
                <GridColumn Title="@item" Width="120px">
                    <FooterTemplate>
                        @if (ColumnAgg is not null && ColumnAgg.ContainsKey(item))
                        {
                            @if (item.Contains("????")) { <span>100</span> }
                            else { <span>@ColumnAgg[item]</span> }
                        }
                    </FooterTemplate>
                </GridColumn>
            }
            @if (AddedPivotColumn is not null)
            {
                <GridColumn Title="@TextResources.APP_StringKeys_SumValue">
                    <FooterTemplate>@TotalSum</FooterTemplate>
                </GridColumn>
            }
        </GridColumns>
        <GridToolBarTemplate>
            <TelerikButton Icon="@FontIcon.FileExcel" OnClick="@OnExcelExportClick" Class="green-excel">
                @TextResources.APP_StringKeys_ExportExcel
            </TelerikButton>
        </GridToolBarTemplate>
    </TelerikGrid>
}

<TelerikLoaderContainer Visible="IsLoading"
                        LoaderPosition="@LoaderPosition.End"
                        LoaderType="LoaderType.InfiniteSpinner"
                        Text="@TextResources.APP_StringKeys_Loading">
</TelerikLoaderContainer>
```

---

## Step 3: Create the Code-Behind File

**`Silo\Pages\DynamicReports\{Name}Dynamic.razor.cs`**

```csharp
using AutoMapper;
using Silo.Application.Dto.Filter;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using Silo.Shared.Components.Report;

namespace Silo.Pages.DynamicReports;

public partial class {Name}Dynamic
{
    #region Private Fields
    private int FilterCount = 1;
    private int ColumnCount = 1;
    private int CalculationColumnCount = 1;
    private int PivotColumnCount = 1;
    #endregion

    #region Public Fields
    public bool IsLoading = true;
    public bool IsInitPageFinished = false;
    public List<object> Results = new();
    public List<string> GridColumns = new() { "????" };
    public Dictionary<string, decimal> ColumnAgg = new();
    public decimal TotalSum = 0;
    #endregion

    #region Filter Collections
    public List<ReportFilterGeneric<{Name}FilterType>> Filters = new();
    public List<ReportFilterGeneric<{Name}FilterType>> ApplyFilters = new();
    #endregion

    #region Column Collections
    public List<ReportColumnGeneric<{Name}ColumnsType>> DataColumns;
    public List<ReportColumnGeneric<{Name}ColumnsType>> AddedDataColumns = new();
    public List<ReportCalculatingColumn<{Name}ColumnsType>> CalculatingColumns;
    public List<ReportCalculatingColumn<{Name}ColumnsType>> AddedCalculatingColumns = new();
    public List<ReportColumnGeneric<{Name}ColumnsType>> PivotColumns;
    public ReportColumnGeneric<{Name}ColumnsType> AddedPivotColumn;
    public List<ReportColumnGeneric<{Name}ColumnsType>> DataMiningElementColumns = new();
    public List<ReportColumnGeneric<{Name}ColumnsType>> AddedDataMiningElementColumns = new();
    #endregion

    #region Component References
    public ReportAllSection<{Name}ColumnsType, {Name}FilterType> ReportAllSectionRef { get; set; }
    #endregion

    #region Parameters
    [Parameter] public int? FormatId { get; set; }
    #endregion

    #region Injected Services
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    #endregion

    protected override async Task SiloInitializer()
    {
        await LoadReferenceData();
        InitColumnsAndFilters();

        if (FormatId is not null)
            await LoadFormat();

        IsInitPageFinished = true;
        IsLoading = false;
    }

    private async Task LoadReferenceData()
    {
        Shifts = await FormalCache.GetShifts();
        Sizes = await FormalCache.GetSizes();
        Lines = await FormalCache.GetLines();
        ProductTypes = await FormalCache.GetTypes();
        ProductBrands = await FormalCache.GetBrands();
        ProductGroups = await FormalCache.GetGroups();
    }

    private void InitColumnsAndFilters()
    {
        InitStaticFilters();
        InitCalculatingColumns();
        InitSelectColumns();
        InitPivotColumns();
        InitDataMiningElementColumns();
    }
}
```

---

## Step 4: Initialize Filters

```csharp
private void InitStaticFilters()
{
    Filters.Add(new()
    {
        Id = FilterCount++, FieldName = "FromDate",
        FieldType = {Name}FilterType.FromDate,
        EqualityType = FilterEqualityType.BiggerThan,
        Type = FilterType.Static, Component = FilterComponent.PersianDate,
        IsLikeCheckboxShown = false, IsFilterShown = true,
        Label = TextResources.APP_StringKeys_FromDate,
        AdditionalData = new() { { "FilterType", "Static" }, { "FilterId", (FilterCount - 1).ToString() } }
    });

    Filters.Add(new()
    {
        Id = FilterCount++, FieldName = "ToDate",
        FieldType = {Name}FilterType.ToDate,
        EqualityType = FilterEqualityType.SmallerThan,
        Type = FilterType.Static, Component = FilterComponent.PersianDate,
        IsLikeCheckboxShown = false, IsFilterShown = true,
        Label = TextResources.APP_StringKeys_ToDate,
        AdditionalData = new() { { "FilterType", "Static" }, { "FilterId", (FilterCount - 1).ToString() } }
    });

    // Dropdown filter
    Filters.Add(new()
    {
        Id = FilterCount++, FieldName = "Size",
        FieldType = {Name}FilterType.Size,
        Type = FilterType.Static, Component = FilterComponent.Drop,
        IsLikeCheckboxShown = false, IsFilterShown = true,
        Label = TextResources.APP_StringKeys_Product_Size,
        Items = Sizes.Select(p => new ReportDataItem() { Label = p.Title, Value = p.Code }).ToList(),
        AdditionalData = new() { { "FilterType", "Static" }, { "FilterId", (FilterCount - 1).ToString() } }
    });
}
```

### Filter Component Reference

| Component | Use Case |
|-----------|----------|
| `FilterComponent.PersianDate` | Date range filters |
| `FilterComponent.Drop` | Fixed dropdown list |
| `FilterComponent.Text` | Free-form text |
| `FilterComponent.Modal` | Large list (warehouses) |
| `FilterComponent.ProductCodeModal` | Product selection |
| `FilterComponent.LocationModal` | Location selection |
| `FilterComponent.Time` | Time picker |

---

## Step 5: Initialize Columns

```csharp
private void InitSelectColumns()
{
    DataColumns = new()
    {
        new ReportColumnGeneric<{Name}ColumnsType>
        {
            Id = ColumnCount++, Title = TextResources.APP_StringKeys_OperationCode,
            Type = {Name}ColumnsType.OperationCode, IsColumnShown = true,
            AdditionalData = new() { { "ColumnType", "Static" }, { "ColumnId", (ColumnCount - 1).ToString() } }
        },
        new ReportColumnGeneric<{Name}ColumnsType>
        {
            Id = ColumnCount++, Title = TextResources.APP_StringKeys_ProductCode,
            Type = {Name}ColumnsType.ProductCode, IsColumnShown = true,
            AdditionalData = new() { { "ColumnType", "Static" }, { "ColumnId", (ColumnCount - 1).ToString() } }
        }
        // Add more columns as needed
    };
}

private void InitCalculatingColumns()
{
    CalculatingColumns = new()
    {
        new ReportCalculatingColumn<{Name}ColumnsType> { Id = CalculationColumnCount++, Title = TextResources.APP_StringKeys_Count, Type = ReportCalculatingColumnType.Count, GroupColumnType = {Name}ColumnsType.ProductSerial },
        new ReportCalculatingColumn<{Name}ColumnsType> { Id = CalculationColumnCount++, Title = TextResources.APP_StringKeys_SumValue, Type = ReportCalculatingColumnType.Sum, GroupColumnType = {Name}ColumnsType.ProductCount },
        new ReportCalculatingColumn<{Name}ColumnsType> { Id = CalculationColumnCount++, Title = TextResources.APP_StringKeys_Average, Type = ReportCalculatingColumnType.Avg, GroupColumnType = {Name}ColumnsType.ProductCount },
        new ReportCalculatingColumn<{Name}ColumnsType> { Id = CalculationColumnCount++, Title = TextResources.APP_StringKeys_Percent, Type = ReportCalculatingColumnType.Percent, GroupColumnType = {Name}ColumnsType.ProductCount }
    };
}

private void InitPivotColumns()
{
    PivotColumns = new()
    {
        new ReportColumnGeneric<{Name}ColumnsType> { Id = PivotColumnCount++, Title = TextResources.APP_StringKeys_Size_Title + "(Pivot)", Type = {Name}ColumnsType.SizeTitle },
        new ReportColumnGeneric<{Name}ColumnsType> { Id = PivotColumnCount++, Title = TextResources.APP_StringKeys_Brand_Title + "(Pivot)", Type = {Name}ColumnsType.BrandTitle }
    };
}

private void InitDataMiningElementColumns()
{
    DataMiningElementColumns = new();
    foreach (var element in DataMiningElements)
    {
        DataMiningElementColumns.Add(new ReportColumnGeneric<{Name}ColumnsType>
        {
            Id = ColumnCount++, Title = element.Title,
            Type = {Name}ColumnsType.DataMiningElements, IsColumnShown = true,
            Value = element.Id.ToString(),
            AdditionalData = new() { { "ColumnType", "DataMiningElement" }, { "ColumnId", element.Id.ToString() } }
        });
    }
}
```

---

## Step 6: Implement Event Handlers

```csharp
// Filter events
public async Task OnFilterRemoveClick(ReportFilterGeneric<{Name}FilterType> filter)
    => ApplyFilters.RemoveAll(p => p.FieldName.Equals(filter.FieldName) && p.Value.Equals(filter.Value));

public async Task OnAddNewFilter(ReportFilterGeneric<{Name}FilterType> filter)
    => ApplyFilters.Add(filter);

// Column events
public async Task OnDataColumnAdd(ReportColumn column)
    => AddedDataColumns.Add((ReportColumnGeneric<{Name}ColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id));

public async Task OnDataColumnRemove(ReportColumn column)
    => AddedDataColumns.RemoveAll(p => p.Id == column.Id);

public async Task OnCalculatingColumnAdd(ReportColumn column)
{
    var calCol = (ReportCalculatingColumn<{Name}ColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);
    AddedCalculatingColumns.Add(new() { GroupColumnType = calCol.GroupColumnType, Id = calCol.Id, Title = calCol.Title, Type = calCol.Type });
}

public async Task OnCalculatingColumnRemove(ReportColumn column)
    => AddedCalculatingColumns.RemoveAll(p => p.Id == column.Id);

public async Task OnPivotColumnAdd(ReportColumn column)
    => AddedPivotColumn = (ReportColumnGeneric<{Name}ColumnsType>)PivotColumns.FirstOrDefault(p => p.Id == column.Id);

public async Task OnPivotColumnRemove(ReportColumn column)
    => AddedPivotColumn = null;

public async Task OnDataMiningElementColumnAdd(ReportColumn column)
    => AddedDataMiningElementColumns.Add((ReportColumnGeneric<{Name}ColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id));

public async Task OnDataMiningElementColumnRemove(ReportColumn column)
    => AddedDataMiningElementColumns.RemoveAll(p => p.Id == column.Id);
```

---

## Step 7: OnSearchClick + Excel Export

```csharp
public async Task OnSearchClick()
{
    IsLoading = true;
    Results = new();
    GridColumns = new() { "????" };
    ColumnAgg = new();
    TotalSum = 0;

    var request = new {Name}ReportQuery
    {
        Filters = ApplyFilters,
        DataColumns = AddedDataColumns,
        CalculatingColumns = AddedCalculatingColumns,
        PivotColumn = AddedPivotColumn,
        DataMiningElementColumns = AddedDataMiningElementColumns
    };

    var result = (await Api.PostAsyncByContext<{Name}ReportVm>(
        "S{Name}Report", new {Name}ReportVmContext())).Value;

    if (result is not null)
    {
        GridColumns = result.Columns;
        Results = result.Data;
        ColumnAgg = result.ColumnAgg;
        TotalSum = result.TotalSum;
    }

    IsLoading = false;
}

public async Task OnExcelExportClick()
{
    IsLoading = true;
    await ExcelExporter.ExportAsync(Results, GridColumns, "{Name}Report");
    IsLoading = false;
}
```

---

## Step 8: Backend API (ReportBusiness)

Add a method to `Silo.Api\Business\ReportBusiness.cs`:

```csharp
public {Name}ReportVm Get{Name}Report(
    List<ReportFilterGeneric<{Name}FilterType>> filters,
    List<ReportColumnGeneric<{Name}ColumnsType>> dataColumns,
    List<ReportCalculatingColumn<{Name}ColumnsType>> calculatingColumns,
    ReportColumnGeneric<{Name}ColumnsType> pivotColumn,
    List<ReportColumnGeneric<{Name}ColumnsType>> dataMiningElementColumns)
{
    // Build dynamic SQL from filters/columns
    // Execute via IDataAccess
    // Return {Name}ReportVm with Columns, Data, ColumnAgg, TotalSum
}
```

---

## Format Save/Load (if FormatId route is used)

When a `FormatId` parameter is present, load the saved format via `ReportAllSectionRef.LoadFormat(formatId)` after initializing filters/columns. The `IsColumnsEditable="FormatId is null"` binding on `ReportAllSection` automatically makes columns read-only when viewing a saved format.
