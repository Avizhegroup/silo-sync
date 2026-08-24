using AutoMapper;
using Silo.Shared.Tools;
using Silo.Application;

namespace Silo.Pages.Reports;
public partial class Register
{
    public bool IsLoading = true;
    public string CompanyName;
    public int FilterCount = 1;
    public GetAllRegisterQuery Request = new();
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductQcsVm> Qcs;
    public List<AddAccountCommand> Users;
    public List<GetAllRegisterVm>? Products;
    public List<GetAllRegisterDetailsVm>? Details;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductBrandVm> ProductBrands;
    public List<GetAllProductGroupVm> ProductGroups;
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
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    //Action Type = 0 - Update april 2025
    public List<string> DynamicFieldRegisterDataColumns = new();


    public Modal ModalDetails { get; set; }
    public Modal FiltersModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
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

        InitFilters();

        IsLoading = false;
    }

    #region Events
    public void OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Products = null;

        Details = null;

        ApplyFilters.Clear();
    }

    public async Task OnClickSearch(MouseEventArgs e)
    {
        IsLoading = true;

        var filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        Products = (await Api.PostAsyncByContext<List<GetAllRegisterVm>>("SRepRegisterTagSummary",
            new GetAllRegisterVmContext(),
            new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickRowDetails(GetAllRegisterVm product)
    {
        IsLoading = true;

        var filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        ReportFilter productCodeFilter = Filters.First(p => p.FieldName.Equals("ProductCode"));

        productCodeFilter.Values = new()
        {
            product.ProductCode
        };

        filters.RemoveAll(p => p.FieldName.Equals("ProductCode"));

        filters.Add(productCodeFilter);

        DynamicFieldRegisterDataColumns = new();

        foreach (var filter in ApplyFilters.Where(p => p.Type == FilterType.Dynamic))
        {
            if (filter.AdditionalData.First(p => p.Key.Equals("DynamicFilterActionType")).Value == "0")
            {
                DynamicFieldRegisterDataColumns.Add(filter.Label);
            }
        }

        foreach (var dynamicField in DynamicFields)
        {
            if (dynamicField.ActionType != 0 || !dynamicField.FieldShowColumn)
            {
                continue;
            }

            DynamicFieldRegisterDataColumns.ReplaceOrAdd(p=>p.Equals(dynamicField.Title), dynamicField.Title);
        }

        Details = (await Api.PostAsyncByContext<List<GetAllRegisterDetailsVm>>("SRepRegisterTagDetails",
           new GetAllRegisterDetailsVmContext(),
           new KeyValuePair<string, object>("reportFilters", filters))).Value;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    public async Task OnClickExportToPdfMaster()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetAllRegisterVm), Products)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration["Settings:Company"];
        }

        List<KeyValuePair<string, object>> variables = new()
        {
             new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
           , new("CompanyName", CompanyName)
           , new("PageTitle", PageTitle)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "Register",
            Variables = variables,
            DataSources = dataSources,
            Images = images
        };

        var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
         , "PreparedReport/Create"
         , command);

        if (response.Value.Result < 1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            return;
        }

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    public async Task OnExcelBeforeExport(GridBeforeExcelExportEventArgs args, string fileName)
    {
        IsLoading = true;

        var dataTable = ExcelExportTools.GetDataTableWithDynamicColumnAndValues(args); 

        var stream = ExcelExporter.ExportDatatable(dataTable);

        stream.Seek(0, SeekOrigin.Begin);

        await Exporter.ExportAndDownload(stream, $"{fileName}.xlsx");

        IsLoading = false;
    }
    #endregion

    #region Filters
    public async Task OnAddNewFilterClick(List<ReportFilter> filters)
    {
        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnFilterRemoveClick(ReportFilter filter)
    {
        ApplyFilters.Remove(filter);
    }
    #endregion

    #region Privates
    private async Task InitFilters()
    {
        Filters.Clear();

        #region Static Filters

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.ProductCode.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.ProductCodeModal,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Chart_ProductCode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.Line.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Line,
            Items = Lines.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.FromDate.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            EqualityType = FilterEqualityType.BiggerThan,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_FromDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.ToDate.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            EqualityType = FilterEqualityType.SmallerThan,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ToDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.User.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_User,
            Items = Users.Select(p => new ReportDataItem()
            {
                Label = p.UserName,
                Value = p.UserName
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.Shift.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Chart_Shift,
            Items = Shifts.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.Qc.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Chart_Qc,
            Items = Qcs.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.TechnicalCode.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Chart_Regcode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.ProductSerial.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_ProductSerial
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.Size.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Product_Size,
            Items = Sizes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.ContractStatus.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_ContractStatus
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.ProductGroup.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ProductGroup,
            Items = ProductGroups.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.ProductBrand.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ProductBrand,
            Items = ProductBrands.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.RegisterDevice.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Register_Device,
            Items = RegisterDevices.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = ReportRegisterFilters.InspectStatus.ToString(),
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Inspect_Status,
            Items = InspectStatus.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList()
        });
        #endregion

        await SetDynamicFiltersByActionType(new() { 0 });
    }

    private async Task SetDynamicFiltersByActionType(List<int> actionTypeIds)
    {
        if (actionTypeIds.Any())
        {
            DynamicFields = new();

            foreach (var actionTypeId in actionTypeIds)
            {
                var dynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", actionTypeId))).Value;

                DynamicFields.AddRange(dynamicFields);
            }

            Filters.RemoveAll(p => p.Type == FilterType.Dynamic);

            foreach (var field in DynamicFields.Where(p => p.FieldType == DynamicFieldType.HeaderData))
            {
                if (field.ValueType == DynamicFieldValueType.TextBox)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Text,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
                else if (field.ValueType == DynamicFieldValueType.DropDown)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        Component = FilterComponent.Drop,
                        Type = FilterType.Dynamic,
                        IsLikeCheckboxShown = false,
                        FieldName = field.Title,
                        Value = field.DefaultValue,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        },
                        Items = field.ValueOptionList.Select(p => new ReportDataItem()
                        {
                            Label = p,
                            Value = p
                        }).ToList()
                    });
                }
                else if (field.ValueType == DynamicFieldValueType.RichTextEditor)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.RichTextEditor,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
                else if (field.ValueType == DynamicFieldValueType.Numeric)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Numeric,
                        IsLikeCheckboxShown = false,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
                else if (field.ValueType == DynamicFieldValueType.Plaque)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Plaque,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
            }

            Filters = Filters.OrderBy(p => p.Type).ToList();
        }
        else
        {
            Filters.RemoveAll(p => p.Type == FilterType.Dynamic);
        }
    }
    #endregion
}
