using Silo.Application;
using Silo.Shared.Components;

namespace Silo.Modules.Guarantee.Pages;
public partial class ExpireAndGuarantee
{
    public bool IsLoading = true;
    public bool IsAllSelected = false;
    public SaveExpireGuaranteeByProductCodesCommand Command = new();
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<GetGuaranteeProductsVm> Products;
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllProductClassVm> Classes;
    public List<GetAllProductSubGroupVm> SubGroups;
    public List<TelerikDropDownItemGeneric<GuaranteeTypes>> GuaranteeItems = new()
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

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        ProductTypes = await FormalCache.GetTypes();

        Groups = await FormalCache.GetGroups();

        Brands = await FormalCache.GetBrands();

        Sizes = await FormalCache.GetSizes();

        Qcs = await FormalCache.GetQcs();


        Classes = await FormalCache.GetProductClass();

        SubGroups = await FormalCache.GetSubGroups();

        InitFilters();

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

        Products = (await Api.PostAsyncByUriAndContext<List<GetGuaranteeProductsVm>>("wms/Product",
            "SGetProductsForGuaranteeExpire",
            new GetGuaranteeProductsVmContext(),
            new KeyValuePair<string, object>("reportFilters", filters))).Value;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (!IsValid())
        {
            return;
        }

        Command.ProductCodes = Products.Where(p => p.IsSelected)
                                       .Select(p => p.ProductCode)
                                       .ToList();

        bool result = (await Api.PostAsyncByUri<bool>("wms/Product"
            , "SSaveExpireAndGuaranteeByProductCode"
            , new KeyValuePair<string, object>("command", Command))).Value;

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

    private void InitFilters()
    {
        Filters.Clear();

        int indexer = 0;

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
            Label = TextResources.APP_StringKeys_ProductName,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ProductName"
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
            Label = TextResources.APP_StringKeys_Product_Size,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "ProductSize",
            Items = Sizes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_QC,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Type = FilterType.Static,
            FieldName = "Qc",
            Items = Qcs.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });
    }

    private bool IsValid()
    {
        if (Command.GuaranteeType == GuaranteeTypes.None)
        {
            if (Command.GuaranteeMonths != 0)
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable
                , TextResources.APP_StringKeys_ExpireAndGuarantee_GuaranteeStatus), "error");

                return false;
            }
        }
        else
        {
            if (Command.GuaranteeType == GuaranteeTypes.Date)
            {
                if (Command.GuaranteeDate.HasNoValue())
                {
                    Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable
                                    , TextResources.APP_StringKeys_ToDate), "error");

                    return false;
                }
            }
            else if (Command.GuaranteeMonths == 0)
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable
                , TextResources.APP_StringKeys_ExpireAndGuarantee_ExpireDays), "error");

                return false;
            }
        }

        if (Command.ExpireType == GuaranteeTypes.None)
        {
            if (Command.ExpireMonths != 0)
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable
               , TextResources.APP_StringKeys_ExpireAndGuarantee_ExpireStatus), "error");

                return false;
            }
        }
        else
        {
            if (Command.ExpireType == GuaranteeTypes.Date)
            {
                if (Command.ExpireDate.HasNoValue())
                {
                    Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable
                                    , TextResources.APP_StringKeys_ToDate), "error");

                    return false;
                }
            }
            else if(Command.ExpireMonths == 0)
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable
                    , TextResources.APP_StringKeys_ExpireAndGuarantee_ExpireDays), "error");

                return false;
            }
        }

        if (!Products.Any(p => p.IsSelected))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return false;
        }

        return true;
    }
}
