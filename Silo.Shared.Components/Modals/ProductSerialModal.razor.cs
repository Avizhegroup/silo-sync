using DocumentFormat.OpenXml.Spreadsheet;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Identity.Client;

namespace Silo.Shared.Components.Modals;
public partial class ProductSerialModal
{
    public bool IsLoading = false;
    public GetAllProductBySerialQuery SearchTagsRequest = new();
    public List<GetAllProductBySerialVm> SearchTags = new();
    public List<GetAllProductBySerialVm> SelectedTags;
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductTypeVm> Types;
    public List<GetAllProductGroupVm> Groups;
    public bool IsAllSelected = false;
    public bool ShowDynamicFields = false;
    public List<GetAllDynamicFieldVm> ProductDynamicFields = new();
    public List<TelerikDropDownItemGeneric<int>> FreezeStatuses = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_NotChoosed,
            Value = -1
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Status_Not_Freezed,
            Value = 0
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Status_Freezed,
            Value = 1
        }
    };

    public Modal Modal { get; set; }

    [Parameter] public bool MultiSelect { get; set; } = true;
    [Parameter] public EventCallback<List<GetAllProductBySerialVm>> OnSelectSerials { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
  
    public async Task Open()
    {
        IsLoading = true;

        ClearProductModal();

        if (Shifts is null)
        {
            Shifts = await FormalCache.GetShifts();
        }

        if (Lines is null)
        {
            Lines = await FormalCache.GetLines();
        }

        if (Qcs is null)
        {
            Qcs = await FormalCache.GetQcs();
        }

        if (Sizes is null)
        {
            Sizes = await FormalCache.GetSizes();
        }

        if (Brands is null)
        {
            Brands = await FormalCache.GetBrands();
        }

        if (Types is null)
        {
            Types = await FormalCache.GetTypes();
        }

        if (Groups is null)
        {
            Groups = await FormalCache.GetGroups();
        }

        if (!ProductDynamicFields.Any())
        {
            var allFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetAllDynamicFields")).Value;

            ProductDynamicFields = allFields?.OrderBy(f => f.Order).ToList();
        }

        IsLoading = false;

        await Modal.Open(new());
    }

    public async Task OnSearchProductModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllProductBySerialQuery request = FixEmptiness();

        SearchTags = (await Api.PostAsync<List<GetAllProductBySerialVm>>("SPSearchProductBySerial",
            new KeyValuePair<string, object>("search", request) )).Value;

        ShowDynamicFields = false;

        IsLoading = false;
    }

    public async Task OnProductModalYes(MouseEventArgs e)
    {
        IsLoading = true;

        AddNewSerials(SearchTags.Where(x => x.IsChoosed == true).ToList());

        IsLoading = false;

        await OnSelectSerials.InvokeAsync(SelectedTags);
    }

    public async Task OnSingleSelectClick(GetAllProductBySerialVm product)
    {
        AddNewSerials(new()
        {
            product
        });

        await Modal.Close(new());

        await OnSelectSerials.InvokeAsync(SelectedTags);
    }

    public async Task OnToggleSelectAll()
    {
        SearchTags.ForEach(p => p.IsChoosed = IsAllSelected);
    }

    public async Task OnToggleSelectChange(object value)
    {
        bool castedValue = (bool)value;

        if (!castedValue)
        {
            IsAllSelected = false;
        }
    }

    private GetAllProductBySerialQuery FixEmptiness()
    {
        GetAllProductBySerialQuery request = new();

        if (SearchTagsRequest.FromSerial.HasNoValue())
        {
            request.FromSerial = "-1";
        }
        else
        {
            request.FromSerial = SearchTagsRequest.FromSerial;
        }

        if (SearchTagsRequest.ToSerial.HasNoValue())
        {
            request.ToSerial = "-1";
        }
        else
        {
            request.ToSerial = SearchTagsRequest.ToSerial;
        }

        if (SearchTagsRequest.Line.HasNoValue())
        {
            request.Line = "-1";
        }
        else
        {
            request.Line = SearchTagsRequest.Line;
        }

        if (SearchTagsRequest.ProductCode.HasNoValue())
        {
            request.ProductCode = "-1";
        }
        else
        {
            request.ProductCode = SearchTagsRequest.ProductCode;
        }

        if (SearchTagsRequest.TechnicalCode.HasNoValue())
        {
            request.TechnicalCode = "-1";
        }
        else
        {
            request.TechnicalCode = SearchTagsRequest.TechnicalCode;
        }

        if (SearchTagsRequest.FromDate.HasNoValue())
        {
            request.FromDate = "-1";
        }
        else
        {
            request.FromDate = SearchTagsRequest.FromDate;
        }

        if (SearchTagsRequest.ToDate.HasNoValue())
        {
            request.ToDate = "-1";
        }
        else
        {
            request.ToDate = SearchTagsRequest.ToDate;
        }

        if (SearchTagsRequest.Shift.HasNoValue())
        {
            request.Shift = "-1";
        }
        else
        {
            request.Shift = SearchTagsRequest.Shift;
        }

        if (SearchTagsRequest.Size.HasNoValue())
        {
            request.Size = "-1";
        }
        else
        {
            request.Size = SearchTagsRequest.Size;
        }

        if (SearchTagsRequest.Qc.HasNoValue())
        {
            request.Qc = "-1";
        }
        else
        {
            request.Qc = SearchTagsRequest.Qc;
        }

        if (SearchTagsRequest.OldSerial.HasNoValue())
        {
            request.OldSerial = "-1";
        }
        else
        {
            request.OldSerial = SearchTagsRequest.OldSerial;
        }

        if (SearchTagsRequest.Brand.HasNoValue())
        {
            request.Brand = "-1";
        }
        else
        {
            request.Brand = SearchTagsRequest.Brand;
        }

        if (SearchTagsRequest.Group.HasNoValue())
        {
            request.Group = "-1";
        }
        else
        {
            request.Group = SearchTagsRequest.Group;
        }

        if (SearchTagsRequest.Type.HasNoValue())
        {
            request.Type = "-1";
        }
        else
        {
            request.Type = SearchTagsRequest.Type;
        }

        request.FreezeStatus = SearchTagsRequest.FreezeStatus;

        request.TechnicalCodeLike = SearchTagsRequest.TechnicalCodeLike;

        request.DynamicFilters = ProductDynamicFields
            .Where(f => !string.IsNullOrWhiteSpace(f.Value))
            .ToDictionary(f => f.Title, f => f.Value);

        return request;
    }

    private void ClearProductModal()
    {
        SearchTagsRequest = new();

        SearchTags = new();

        SelectedTags = new();

        IsAllSelected = false;

        ProductDynamicFields.ForEach(f => f.Value = string.Empty);
    }

    private void AddNewSerials(List<GetAllProductBySerialVm> newProducts)
    {
        SelectedTags ??= new();

        SelectedTags.AddRange(newProducts);

        SelectedTags = SelectedTags.DistinctBy(product => product.ProductSerial).ToList();
    }
}
