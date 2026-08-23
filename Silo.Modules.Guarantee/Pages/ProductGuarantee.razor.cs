using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Silo.Application;
using Silo.Identity.Client;
using Silo.Shared.Components;

namespace Silo.Modules.Guarantee.Pages;
public partial class ProductGuarantee
{
    public bool IsLoading = true;
    public bool IsAllSelected = false;
    public string UserName;
    public string CompanyName;
    public SaveProductGuaranteesCommand Command = new();
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<GetProductGuaranteesVm> Products;
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductClassVm> Classes;
    public List<GetAllProductSubGroupVm> SubGroups;
    public List<TelerikDropDownItemGeneric<string>> SerialGuaranteeStatusItems = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_NotStarted,
            Value = ((int)SerialGuaranteeStatus.NotStarted).ToString()
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Active,
            Value = ((int)SerialGuaranteeStatus.Active).ToString()
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Finished,
            Value = ((int)SerialGuaranteeStatus.Finished).ToString()
        }
    };
    public List<TelerikDropDownItemGeneric<GuaranteeTypes>> GuaranteeActivationTypes = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_NotChoosed,
            Value = GuaranteeTypes.None
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Enter,
            Value = GuaranteeTypes.EnterToWarehouse
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Exit,
            Value = GuaranteeTypes.ExitFromWarehouse
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Inspect,
            Value = GuaranteeTypes.AcceptInspect
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Factory,
            Value = GuaranteeTypes.ExitFromFactory
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Sell,
            Value = GuaranteeTypes.Sell
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Install,
            Value = GuaranteeTypes.Install
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Customer,
            Value = GuaranteeTypes.Customer
        },
        new()
        {
            Name = TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Date,
            Value = GuaranteeTypes.Date
        }
    };

    public Modal FiltersModal { get; set; }

    [Parameter] public int? ExitActionCode { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    protected override async Task SiloInitializer()
    {
        UserName = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserPersianName();

        ProductTypes = await FormalCache.GetTypes();


        Groups = await FormalCache.GetGroups();

        Brands = await FormalCache.GetBrands();

        Classes = await FormalCache.GetProductClass();

        SubGroups = await FormalCache.GetSubGroups();

        InitFilters();

        if (ExitActionCode is not null)
        {
            ApplyFilters.Add(new()
            {
                AddType = FilterAddType.And,
                Component = FilterComponent.Text,
                Label = TextResources.APP_StringKeys_ExitAction_Code,
                Type = FilterType.Static,
                IsLikeCheckboxShown = true,
                FieldName = "ExitActionCode",
                Value = ExitActionCode.ToString()
            });

            await OnSearchClick();
        }

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;

        ApplyFilters = new();

        Command = new();

        Products = null;
    }

    public async Task OnSearchClick()
    {
        IsLoading = true;

        var filters = ApplyFilters.GroupBy(p => p.FieldName)
                      .Select(p => new
                                  ReportFilter()
                      {
                          FieldName = p.Key,
                          Type = p.First().Type,
                          Component = p.First().Component,
                          EqualityType = p.First().EqualityType,
                          AddType = p.First().AddType,
                          Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                      }).ToList();

        Products = (await Api.PostAsyncByUriAndContext<List<GetProductGuaranteesVm>>("wms/Product",
            "SGetProductGuarantees",
            new GetProductGuaranteesVmContext(),
            new KeyValuePair<string, object>("reportFilters", filters))).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (!IsValid())
        {
            return;
        }

        IsLoading = true;

        Command.ProductSerials = Products.Where(p => p.IsSelected)
                                       .Select(p => p.ProductSerial)
                                       .ToList();

        bool result = (await Api.PostAsyncByUri<bool>("wms/Product"
            , "SSaveProductGuarantees"
            , new KeyValuePair<string, object>("command", Command))).Value;

        IsLoading = false;

        if (result)
        {
            Command = new();

            Products = new();

            Filters.Clear();

            await OnSearchClick();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    #region Toggle
    public async Task OnToggleSelectAll()
    {
        if (Products is not null)
        {
            Products.ForEach(p => p.IsSelected = IsAllSelected);
        }
    }

    public async Task OnToggleSelectChange(object value)
    {
        bool castedValue = (bool)value;

        if (!castedValue)
        {
            IsAllSelected = false;
        }
    }
    #endregion

    #region Filters
    public async void OnAddNewFilterClick(List<ReportFilter> filters)
    {
        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnFilterRemoveClick(ReportFilter filter)
    {
        ApplyFilters.Remove(filter);
    }

    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        Filters = new();

        InitFilters();

        await FiltersModal.Open(e);
    }
    #endregion

    #region Export
    public async Task OnExportToPdfClick()
    {
        if (!IsValidPrint())
        {
            return;
        }

        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        List<KeyValuePair<string, object>> variables = new()
        {
            new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}"),
            new("User", UserName),
            new("CompanyName", CompanyName),
            new("GuaranteeStatus", SerialGuaranteeStatusItems.First(p=>p.Value == Command.GuaranteeStatus).Name),
            new("GuaranteeStartDate",Command.GuaranteeStartDate),
            new("GuaranteedEndDate",Command.GuaranteedEndDate),
            new("PageTitle", PageTitle)
        };

        List<PrintProductGuaranteesDto> productGuarantees = Mapper.Map<List<PrintProductGuaranteesDto>>(Products.Where(p=>p.IsSelected));

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(PrintProductGuaranteesDto), productGuarantees)
        }; 

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "ProductGuarantee",
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
    #endregion

    private void InitFilters()
    {
        Filters.Clear();

        int indexer = 0;

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductType,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "ProductType",
            Items = ProductTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductGroup,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "ProductGroup",
            Items = Groups.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_Product_SubGroup,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "ProductSubGroup",
            Items = SubGroups.Select(p => new ReportDataItem()
            {
                Label = p.Title + " / " + "گروه: " + p.ProductGroupTitle,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductClass,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "ProductClass",
            Items = Classes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_Chart_Regcode,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "TechnicalCode"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductName,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ProductName"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductCode,
            IsLikeCheckboxShown = true,
            Type = FilterType.Static,
            Component = FilterComponent.ProductCodeModal,
            FieldName = "ProductCode"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductBrand,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "ProductBrand",
            Items = Brands.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductSerial,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "ProductSerial"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "GuaranteeStatus",
            Items = SerialGuaranteeStatusItems.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ExitAction_Date,
            Component = FilterComponent.PersianDate,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.Equals,
            IsLikeCheckboxShown = false,
            FieldName = "ExitActionDateTime"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ExitAction_DocumentCode,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ExitActionDocumentCode"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ExitAction_Code,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ExitActionCode"
        });
    }

    private bool IsValid()
    {
        if (!Products.Any(p => p.IsSelected))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return false;
        }

        return true;
    }

    private bool IsValidPrint()
    {
        if(Products is null)
        {
            Notification.Show(TextResources.APP_StringKeys_Data_NotFound_Print, "error");

            return false;
        }

        if (Products.Neither(p=>p.IsSelected))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return false;
        }

        var context = new ValidationContext(Command, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(Command, context, results);

        if (!isValid)
        {
            foreach (var validationResult in results)
            {
                Notification.Show(validationResult.ErrorMessage, "error");
            }
            return false;
        }

        return true;
    }
}
