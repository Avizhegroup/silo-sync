using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Application.Dto.DynamicField;
using Silo.Application.Features;
using Silo.Components.DynamicField;
using Silo.Identity.Client;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Inspect.Pages;
public partial class InspectStaticReport
{
    public bool IsLoading = true;
    public bool IsSmallerScreen = false;
    public string FilteredElements = "";
    public GetAllInspectReportQuery Request = new();
    public List<string> Options = new();
    public List<UserDropDownableDto> Users;
    public List<GetAllInspectElementVm> InspectElements;
    public List<GetAllLinesVm> Lines;
    public List<GetAllInspectReportVm> Inspects;
    public GetAllInspectReportVm InspectChoosed = new();
    public List<TelerikDropDownItemGeneric<int>> InspectResults = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Verified,
            Value = (int) InspectResult.Verfied
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Unverified,
            Value = (int) InspectResult.NotVerfied
        }
    };
    public List<TelerikDropDownItem> OneOptionValues = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Verified,
            Value = "true"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Unverified,
            Value = "false"
        }
    };
    public List<DynamicFieldWithValueDto> DynamicFieldsDto = new();

    public DynamicFieldFillValue DynamicFieldFillValueRef { get; set; }

    [Inject] public IMapper Mapper { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal ModalElementFilter { get; set; }
    public Modal ModalInspectDetails { get; set; }
    public ProductCodeModal ProductCodeModal { get; set; }
    public TelerikDropDownList<string, string> ComboMultiOptions { get; set; }


    protected override async Task SiloInitializer()
    {
        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        await RefreshElementData();

        Users = Mapper.Map<List<ApplicationUser>, List<UserDropDownableDto>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        Lines = await FormalCache.GetLines();
        var dynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
                             new KeyValuePair<string, object>("actionTypeId", 1))).Value;

        DynamicFieldsDto = dynamicFields.DistinctBy(p => p.Title)
                                        .Select(p => new DynamicFieldWithValueDto()
                                        {
                                            Title = p.Title,
                                            DefaultValue = p.DefaultValue,
                                            Value = p.DefaultValue,
                                            ValueOptions = p.ValueOptionList,
                                            ValueType = p.ValueType,
                                            IsReadOnly = p.IsReadOnly ?? false
                                        }).ToList();

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Inspects = null;

        FilteredElements = "";

        InspectElements.ForEach(element => element.Value = null);

        await DynamicFieldFillValueRef.Clear();
    }

    public async Task OnClickSubmit(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllInspectReportQuery request = await FixEmptiness();

        Inspects = (await Api.PostAsync<List<GetAllInspectReportVm>>("SReportInspects"
            , new KeyValuePair<string, object>("request", request))).Value;

        IsLoading = false;

        IsFiltersShown = false;
    }

    public async Task OnClickProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public async Task OnClickModalYes(MouseEventArgs e)
    {
        FilteredElements = "";

        foreach (GetAllInspectElementVm element in InspectElements.Where(element => element.Value.HasValue()).ToList())
        {
            FilteredElements += $"{element.Name}=" + element.InspectElementType switch
            {
                InspectElementType.OneOption => element.Value.Equals("true") ? TextResources.APP_StringKeys_Verified : TextResources.APP_StringKeys_Unverified,
               _ => element.Value
            } + ", ";
        }

        FilteredElements = FilteredElements.Length > 2 ? FilteredElements.Remove(FilteredElements.Length - 2): string.Empty;
    }

    public async Task RefreshElementData()
    {
        IsLoading = true;

        InspectElements = (await Api.PostAsync<List<GetAllInspectElementVm>>("SGetAllElements")).Value;
        InspectElements.ForEach(element => element.Value = null);
        IsLoading = false;
    }

    public async Task OnClickRowDetails(GetAllInspectReportVm inspect)
    {
        InspectChoosed = inspect;

        await ModalInspectDetails.Open(new());
    }

    private async Task<GetAllInspectReportQuery> FixEmptiness()
    {
        GetAllInspectReportQuery request = new();

        request.ProductSerial = "-1";

        if (Request.FromDate.HasValue())
        {
            request.FromDate = Request.FromDate;
        }
        else
        {
            request.FromDate = "-1";
        }

        if (Request.ToDate.HasValue())
        {
            request.ToDate = Request.ToDate;
        }
        else
        {
            request.ToDate = "-1";
        }

        if (Request.ProductCode.HasValue())
        {
            request.ProductCode = Request.ProductCode;
        }
        else
        {
            request.ProductCode = "-1";
        }

        if (Request.RegCode.HasValue())
        {
            request.RegCode = Request.RegCode;
        }
        else
        {
            request.RegCode = "-1";
        }

        if (Request.Line.HasValue())
        {
            request.Line = Request.Line;
        }
        else
        {
            request.Line = "-1";
        }

        if (Request.InspectResult != -1)
        {
            request.InspectResult = Request.InspectResult;
        }
        else
        {
            request.InspectResult = -1;
        }

        if (Request.UserId.HasValue())
        {
            request.UserId = Request.UserId;
        }
        else
        {
            request.UserId = "-1";
        }

        List<GetAllInspectElementVm> filterElements = InspectElements.Where(element => element.Value.HasValue()).ToList();

        if (filterElements.Any())
        {
            filterElements.ForEach(element => request.ElementFilters.Add(new () 
            {
                InspectElementId = element.Id, 
                InspectElementValue = element.Value 
            }));
        }
        else
        {
            request.ElementFilters = new();
        }

        request.DynamicFilters = (await DynamicFieldFillValueRef.GetKeyValueList()).Where(p=>p.Value.HasValue()).ToList();

        return request;
    }
}
