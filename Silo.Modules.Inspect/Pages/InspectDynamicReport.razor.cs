using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Linq;
using Silo.Identity.Client;
using Silo.Shared.Components;

namespace Silo.Modules.Inspect.Pages;
public partial class InspectDynamicReport
{
    public bool IsLoading = true;
    public string UserId;
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductQcsVm> Qcs;
    public List<AddAccountCommand> Users;
    public List<object> Results;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductBrandVm> ProductBrands;
    public List<GetAllProductGroupVm> ProductGroups;
    public List<GetAllInspectElementVm> InspectElements;
    public List<TelerikDropDownItem> RegisterDevices = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Kiosk,
            Value = "0"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Handheld,
            Value = ""
        }
    };
    public List<TelerikDropDownItem> InspectStatus = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Inspect_Status_Failed,
            Value = "0"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Inspect_Status_Accept,
            Value = "1"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Inspect_Status_Not,
            Value = "2"
        }
    };
    public List<ReportFilterGeneric<InspectReportDynamicFilterType>> Filters;
    public List<ReportFilterGeneric<InspectReportDynamicFilterType>> ApplyFilters = new();
    public List<ReportColumnGeneric<InspectReportDynamicColumnsType>> DataColumns;
    public List<ReportColumnGeneric<InspectReportDynamicColumnsType>> AddedDataColumns = new();
    public List<ReportCalculatingColumn<InspectReportDynamicColumnsType>> CalculatingColumns;
    public List<ReportCalculatingColumn<InspectReportDynamicColumnsType>> AddedCalculatingColumns = new();
    public List<ReportColumnGeneric<InspectReportDynamicColumnsType>> PivotColumns;
    public ReportColumnGeneric<InspectReportDynamicColumnsType> AddedPivotColumn;
    public List<string> GridColumns = new();
    public Dictionary<string, decimal> ColumnAgg = new();
    private int ColumnCount = 1;
    private int FilterCount = 1;
    private int PivotCount = 1;
    public decimal TotalSum = 0;

    public Modal ModalFilters { get; set; }
    public Modal ModalColumns { get; set; }

    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
   
    protected override async Task SiloInitializer()
    {
        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<AddAccountCommand>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        Shifts = await FormalCache.GetShifts();

        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Lines = await FormalCache.GetLines();

        ProductBrands = await FormalCache.GetBrands();

        ProductGroups = await FormalCache.GetGroups();

        InspectElements = (await Api.PostAsync<List<GetAllInspectElementVm>>("SGetAllElements")).Value;

        AddFilters();

        AddDataColumns();

        AddCalculationColumns();

        AddPivotColumns();

        IsLoading = false;
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        if (!AddedDataColumns.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Dynamic_Column, "error");

            return;
        }

        IsLoading = true;

        TotalSum = 0;

        Results = (await Api.PostAsyncByUri<List<object>>("wms/Inspect"
            , "SInspectReportDynamic"
            , new("filters", ApplyFilters)
            , new("selectColumns", AddedDataColumns)
            , new("calculating", AddedCalculatingColumns)
            , new("pivot", AddedPivotColumn)
            )).Value;

        if (Results.Any())
        {
            AddPivotDataColumns(Results);
        }

        IsLoading = false;

        IsFiltersShown = false;
    }

    public async Task OnExcelExportClick(MouseEventArgs e)
    {
        await ExcelExporter.ExportJsonData(PageTitle, Results, GridColumns);
    }

    #region Filters
    public async Task OnFilterRemoveClick(ReportFilterGeneric<InspectReportDynamicFilterType> filter)
    {
        ApplyFilters.Remove(filter);
    }

    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        ApplyFilters = new();

        await ModalFilters.Open(e);

        StateHasChanged();
    }

    public async Task OnAddNewFilter(ReportFilterGeneric<InspectReportDynamicFilterType> filter)
    {
        ApplyFilters.Add(filter);
    }
    #endregion

    #region Columns
    public async Task OnCalculatingColumnAdd(ReportColumn column)
    {
        var calCol = (ReportCalculatingColumn<InspectReportDynamicColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedCalculatingColumns.Add(new()
        {
            GroupColumnType = calCol.GroupColumnType,
            Id = calCol.Id,
            Title = calCol.Title,
            Type = calCol.Type
        });
    }

    public async Task OnDataColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<InspectReportDynamicColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataColumns.Add(col);
    }

    public async Task OnPivotColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<InspectReportDynamicColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedPivotColumn = col;
    }

    public async Task OnDataColumnRemove(ReportColumn column)
    {
        var col = (ReportColumnGeneric<InspectReportDynamicColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataColumns.RemoveAll(p=>p.Id == col.Id);
    }

    public async Task OnCalculatingColumnRemove(ReportColumn column)
    {
        var calCol = (ReportCalculatingColumn<InspectReportDynamicColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedCalculatingColumns.RemoveAll(p=> p.Id == calCol.Id);
    }

    public async Task OnPivotColumnRemove()
    {
        AddedPivotColumn = null;
    }

    public async Task OnColumnModalClick(MouseEventArgs e)
    {
        await ModalColumns.Open(e);
    }

    public async Task OnClearColumnsClick(MouseEventArgs e)
    {
        AddedCalculatingColumns.Clear();

        AddedDataColumns.Clear();

        AddedPivotColumn = null;

        Results.Clear();
    }
    #endregion

    #region Privates
    private void AddFilters()
    {
        Filters = new()
        {
            new()
            {
                Id =FilterCount++,
                Component = FilterComponent.ProductCodeModal,
                FieldName = "ProductCode",
                Label = TextResources.APP_StringKeys_ProductCode,
                Type = FilterType.Static,
                FieldType = InspectReportDynamicFilterType.ProductCode
            },
            new()
            {
                Id =FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Qc",
                Label = TextResources.APP_StringKeys_ProductStatus,
                Type = FilterType.Static,
                Items = Qcs.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.Qc
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "User",
                Label = TextResources.APP_StringKeys_Inspect_User,
                Type = FilterType.Static,
                Items = Users.Select(p=> new ReportDataItem
                {
                    Label = p.Name,
                    Value = p.UserName,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.User
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Shift",
                Label = TextResources.APP_StringKeys_Chart_Shift,
                Type = FilterType.Static,
                Items = Shifts.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.Shift
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Size",
                Label = TextResources.APP_StringKeys_Product_Size,
                Type = FilterType.Static,
                Items = Sizes.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.Size
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Text,
                FieldName = "TechnicalCode",
                Label = TextResources.APP_StringKeys_Chart_Regcode,
                Type = FilterType.Static,
                FieldType = InspectReportDynamicFilterType.TechnicalCode
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Text,
                FieldName = "ProductSerial",
                Label = TextResources.APP_StringKeys_ProductSerial,
                Type = FilterType.Static,
                FieldType = InspectReportDynamicFilterType.ProductSerial
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.PersianDate,
                FieldName = "FromDate",
                EqualityType = FilterEqualityType.BiggerThan,
                Label = TextResources.APP_StringKeys_FromDate,
                Type = FilterType.Static,
                FieldType = InspectReportDynamicFilterType.FromDate
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.PersianDate,
                FieldName = "ToDate",
                EqualityType = FilterEqualityType.SmallerThan,
                Label = TextResources.APP_StringKeys_ToDate,
                Type = FilterType.Static,
                FieldType = InspectReportDynamicFilterType.ToDate
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "ProductBrand",
                Label = TextResources.APP_StringKeys_ProductBrand,
                Type = FilterType.Static,
                Items = ProductBrands.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.ProductBrand
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "ProductGroup",
                Label = TextResources.APP_StringKeys_ProductGroup,
                Type = FilterType.Static,
                Items = ProductGroups.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.ProductGroup
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Line",
                Label = TextResources.APP_StringKeys_Line,
                Type = FilterType.Static,
                Items = Lines.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.Line
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "RegisterDevice",
                Label = TextResources.APP_StringKeys_RegisterDevice,
                Type = FilterType.Static,
                Items = RegisterDevices.Select(p=> new ReportDataItem
                {
                    Label = p.Name,
                    Value = p.Value,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.RegisterDevice
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "InspectStatus",
                Label = TextResources.APP_StringKeys_Inspect_Status,
                Type = FilterType.Static,
                Items = InspectStatus.Select(p=> new ReportDataItem
                {
                    Label = p.Name,
                    Value = p.Value,
                    IsChoosen = false
                }).ToList(),
                FieldType = InspectReportDynamicFilterType.InspectStatus
            }
        };

        foreach (var field in InspectElements)
        {
            switch (field.InspectElementType)
            {
                case InspectElementType.NotSpecified:
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Name,
                        FieldName = field.Id.ToString(),
                        Type = FilterType.InspectElement,
                        FieldType = InspectReportDynamicFilterType.InspectElements,
                        Component = FilterComponent.Text
                    });
                    break;

                case InspectElementType.MultiOption:
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Name,
                        FieldName = field.Id.ToString(),
                        Type = FilterType.InspectElement,
                        FieldType = InspectReportDynamicFilterType.InspectElements,
                        Component = FilterComponent.Drop,
                        Items = field.Options.Select(p => new ReportDataItem
                        {
                            Label = p,
                            Value = p,
                            IsChoosen = false
                        }).ToList()
                    });
                    break;

                case InspectElementType.OneOption:
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Name,
                        FieldName = field.Id.ToString(),
                        Type = FilterType.InspectElement,
                        FieldType = InspectReportDynamicFilterType.InspectElements,
                        Component = FilterComponent.Drop,
                        Items = new()
                        {
                            new()
                            {
                                Label = TextResources.APP_StringKeys_Verified,
                                Value = "true"
                            },
                            new()
                            {
                                Label = TextResources.APP_StringKeys_Unverified,
                                Value = "false"
                            }
                        }
                    });

                    break;

                case InspectElementType.Int:
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Name,
                        FieldName = field.Id.ToString(),
                        Type = FilterType.InspectElement,
                        FieldType = InspectReportDynamicFilterType.InspectElements,
                        Component = FilterComponent.Text
                    });

                    break;

                case InspectElementType.String:
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Name,
                        FieldName = field.Id.ToString(),
                        Type = FilterType.InspectElement,
                        FieldType = InspectReportDynamicFilterType.InspectElements,
                        Component = FilterComponent.Text
                    });

                    break;

                default:
                    break;
            }
        }
    }

    private void AddCalculationColumns()
    {
        CalculatingColumns = new()
        {
            new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 1,
                Title = TextResources.APP_StringKeys_SumValue,
                Type = ReportCalculatingColumnType.Sum ,
                GroupColumnType = InspectReportDynamicColumnsType.ProductCount
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 2,
                Title = TextResources.APP_StringKeys_Count,
                Type = ReportCalculatingColumnType.Count ,
                GroupColumnType = InspectReportDynamicColumnsType.ProductSerial
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 3,
                Title = TextResources.APP_StringKeys_Percent,
                Type = ReportCalculatingColumnType.Percent ,
                GroupColumnType = InspectReportDynamicColumnsType.ProductCount
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 4,
                Title = TextResources.APP_StringKeys_Max_Count,
                Type = ReportCalculatingColumnType.Max ,
                GroupColumnType = InspectReportDynamicColumnsType.ProductCount
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 5,
                Title = TextResources.APP_StringKeys_Min_Count,
                Type = ReportCalculatingColumnType.Min ,
                GroupColumnType = InspectReportDynamicColumnsType.ProductCount
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 6,
                Title = TextResources.APP_StringKeys_Average,
                Type = ReportCalculatingColumnType.Avg ,
                GroupColumnType = InspectReportDynamicColumnsType.ProductCount
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 7,
                Title = TextResources.APP_StringKeys_Date_Min,
                Type = ReportCalculatingColumnType.Min ,
                GroupColumnType = InspectReportDynamicColumnsType.InspectDate
            },
             new ReportCalculatingColumn<InspectReportDynamicColumnsType>()
            {
                Id = 8,
                Title = TextResources.APP_StringKeys_Date_Max,
                Type = ReportCalculatingColumnType.Max ,
                GroupColumnType = InspectReportDynamicColumnsType.InspectDate
            }
        };
    }

    private void AddDataColumns()
    {
        DataColumns = new()
        {
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductName,
                Type = InspectReportDynamicColumnsType.ProductName,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductCode,
                Type = InspectReportDynamicColumnsType.ProductCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductSerial,
                Type = InspectReportDynamicColumnsType.ProductSerial,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_ProductCount,
                Type = InspectReportDynamicColumnsType.ProductCount,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Qc_Code,
                Type = InspectReportDynamicColumnsType.QcCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Qc_Title,
                Type = InspectReportDynamicColumnsType.QcTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_ProductType_Code,
                Type = InspectReportDynamicColumnsType.TypeCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_ProductType_Title,
                Type = InspectReportDynamicColumnsType.TypeTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Size_Code,
                Type = InspectReportDynamicColumnsType.SizeCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Size_Title,
                Type = InspectReportDynamicColumnsType.SizeTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Line_Code_Salon,
                Type = InspectReportDynamicColumnsType.LineCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Line_Title_Salon,
                Type = InspectReportDynamicColumnsType.LineTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Chart_Regcode,
                Type = InspectReportDynamicColumnsType.Regcode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_DocCode,
                Type = InspectReportDynamicColumnsType.DocCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_RegisterDevice,
                Type = InspectReportDynamicColumnsType.RegisterDevice,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Group_Code,
                Type = InspectReportDynamicColumnsType.GroupCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Group_Title,
                Type = InspectReportDynamicColumnsType.GroupTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Brand_Code,
                Type = InspectReportDynamicColumnsType.BrandCode,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Brand_Title,
                Type = InspectReportDynamicColumnsType.BrandTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Freeze_Status,
                Type = InspectReportDynamicColumnsType.FreezeStatus,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Inspect_Status,
                Type = InspectReportDynamicColumnsType.InspectStatus,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Inspect_User,
                Type = InspectReportDynamicColumnsType.InspectUser,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_Register_User,
                Type = InspectReportDynamicColumnsType.RegisterUser,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_PersianDate_Full,
                Type = InspectReportDynamicColumnsType.PersianDateFull,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_PersianDate_Year,
                Type = InspectReportDynamicColumnsType.PersianDateYear,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_PersianDate_Month,
                Type = InspectReportDynamicColumnsType.PersianDateMonth,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_GregorianDate_Full,
                Type = InspectReportDynamicColumnsType.GregorianDateFull,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_GregorianDate_Year,
                Type = InspectReportDynamicColumnsType.GregorianDateYear,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount ++,
                Title = TextResources.APP_StringKeys_GregorianDate_Month,
                Type = InspectReportDynamicColumnsType.GregorianDateMonth,
            }
        };

        foreach (var field in InspectElements)
        {
            DataColumns.Add(new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = ColumnCount++,
                Title = field.Name,
                Type = InspectReportDynamicColumnsType.InspectElement,
                Value = field.Id.ToString()
            });
        }

    }

    private void AddPivotColumns()
    {
        PivotColumns = new()
        {
            new ReportColumnGeneric<InspectReportDynamicColumnsType>
            {
                Id = PivotCount++,
                Title = TextResources.APP_StringKeys_Size_Title + "(Pivot)",
                Type = InspectReportDynamicColumnsType.SizeTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>
            {
                Id = PivotCount++,
                Title = TextResources.APP_StringKeys_Brand_Title + "(Pivot)",
                Type = InspectReportDynamicColumnsType.BrandTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>
            {
                Id = PivotCount++,
                Title = TextResources.APP_StringKeys_Group_Title + "(Pivot)",
                Type = InspectReportDynamicColumnsType.GroupTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>
            {
                Id = PivotCount++,
                Title = TextResources.APP_StringKeys_ProductType_Title + "(Pivot)",
                Type = InspectReportDynamicColumnsType.TypeTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>
            {
                Id = PivotCount++,
                Title = TextResources.APP_StringKeys_Qc_Title + "(Pivot)",
                Type = InspectReportDynamicColumnsType.QcTitle,
            },
            new ReportColumnGeneric<InspectReportDynamicColumnsType>
            {
                Id = PivotCount++,
                Title = TextResources.APP_StringKeys_Line_Title_Salon + "(Pivot)",
                Type = InspectReportDynamicColumnsType.LineTitle,
            }
        };

        foreach (var field in InspectElements)
        {
            PivotColumns.Add(new ReportColumnGeneric<InspectReportDynamicColumnsType>()
            {
                Id = PivotCount++,
                Title = field.Name + "(Pivot)",
                Type = InspectReportDynamicColumnsType.InspectElement,
                Value = field.Id.ToString()
            });
        }

    }

    private void AddPivotDataColumns(List<object> data)
    {
        GridColumns.Clear();

        ColumnAgg.Clear();

        var elementFirst = (JsonElement)data.First();

        var jobject = JObject.Parse(elementFirst.GetRawText());
        var names = jobject.Root.Cast<JProperty>()
                                .Select(x => x.Name);

        foreach (var item in AddedDataColumns)
        {
            if (!GridColumns.Any(p => p.Equals(item)))
            {
                GridColumns.Add(item.Title);
            }
        }

        foreach (var item in AddedCalculatingColumns)
        {
            if (!GridColumns.Any(p => p.Equals(item)))
            {
                GridColumns.Add(item.Title);
                ColumnAgg.Add(item.Title, 0);
            }
        }

        foreach (var item in names)
        {
            if (!GridColumns.Any(p => p.Equals(item)))
            {
                GridColumns.Add(item);

                ColumnAgg.Add(item, 0);
            }
        }

        foreach (var item in data)
        {
            var element = (JsonElement)item;

            foreach (var column in ColumnAgg.Keys)
            {
                if (ColumnAgg.ContainsKey(column))
                {
                    if (element.TryGetProperty(column, out var value))
                    {
                        if (decimal.TryParse(value.ToString(), out var result))
                        {
                            ColumnAgg[column] += decimal.Parse(value.ToString().Replace("{}", "0"));
                        }
                    }
                }
            }
        }
    }
    #endregion
}
