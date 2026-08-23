using AutoMapper;
using Silo.Application.Features;


namespace Silo.Pages.Reports;
public partial class Mission
{
    public bool IsLoading = false;
    public bool IsZoneModalForFrom = false;
    public List<TelerikDropDownItem> ProductStatuses = new()
    {
        new()
        {
            Name = "بسته بندی",
            Value = "0"
        },
        new()
        {
            Name = "انبار محصول",
            Value = "1"
        },
        new()
        {
            Name = "خروج از انبار",
            Value = "2"
        }
    };
    public List<TelerikDropDownItem> MissionTypes = new()
    {
        new()
        {
            Name = "جانمایی کالا",
            Value = "1"
        },
        new()
        {
            Name = "جابجایی کالا",
            Value = "2"
        },
        new()
        {
            Name = "جمع آوری کالا",
            Value = "3"
        }
    };
    public List<TelerikDropDownItem> MissionStatuses = new()
    {
        new()
        {
            Name = "اجرا نشده",
            Value = "0"
        },
        new()
        {
            Name = "در حال اجرا",
            Value = "1"
        },
        new()
        {
            Name = "اتمام عملیات",
            Value = "2"
        }
    };
    public List<GetMissionVM> Missions;
    public List<UserDropDownableDto> Drivers;

    public GetMissionQuery Request { get; set; } = new();
    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    protected override async Task SiloInitializer()
    {
        IsLoading = true;

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUserByRole"
                , new("role", "Truck")
                , new("userToken", "Ceramic client user"))).Value;

        Drivers = Mapper.ProjectTo<UserDropDownableDto>(
                 applicationUsers.AsQueryable().Where(p => p.IsActive)).ToList();

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Missions = null;
    }

    public async Task OnClickSearch(MouseEventArgs e)
    {
        IsLoading = true;

        GetMissionQuery request = FillEmptiness();

        Missions = (await Api.PostAsyncByContext<List<GetMissionVM>>("SRepPlacementMissions"
            , new GetMissionVMObjectContext()
            , new("ProductCode", request.ProductCode)
            , new("TechnicalCode", request.TechnicalCode)
            , new("ProductSerial", request.ProductSerial)
            , new("FromDate", request.FromDate)
            , new("ToDate", request.ToDate)
            , new("FromZoneId", request.FromZone)
            , new("ToZoneId", request.ToZone)
            , new("DriverUserId", request.Driver)
            , new("PlacementMissionType", request.MissionType)
            , new("PPMActionId", request.MissionCode)
            , new("TagStatus", request.ProductStatus)
            , new("PlacementMissionStatus", request.MissionStatus))).Value;

        IsLoading = false;
    }

    public async Task OnClickLocation(string code)
    {
        if (IsZoneModalForFrom)
        {
            Request.FromZone = code;
        }
        else
        {
            Request.ToZone = code;
        }
    }

    public async Task OnClickChooseLocation(bool isForFrom)
    {
        IsZoneModalForFrom = isForFrom;

        await LocationModal.Show();
    }

    private GetMissionQuery FillEmptiness()
    {
        GetMissionQuery request = new();

        if (string.IsNullOrEmpty(Request.ProductCode))
        {
            request.ProductCode = string.Empty;
        }
        else
        {
            request.ProductCode = Request.ProductCode;
        }

        if (string.IsNullOrEmpty(Request.TechnicalCode))
        {
            request.TechnicalCode = string.Empty;
        }
        else
        {
            request.TechnicalCode = Request.TechnicalCode;
        }

        request.TechnicalCodeLike = Request.TechnicalCodeLike;

        if (string.IsNullOrEmpty(Request.ProductSerial))
        {
            request.ProductSerial = string.Empty;
        }
        else
        {
            request.ProductSerial = Request.ProductSerial;
        }

        if (string.IsNullOrEmpty(Request.FromDate))
        {
            request.FromDate = string.Empty;
        }
        else
        {
            request.FromDate = Request.FromDate;
        }

        if (string.IsNullOrEmpty(Request.ToDate))
        {
            request.ToDate = string.Empty;
        }
        else
        {
            request.ToDate = Request.ToDate;
        }

        if (string.IsNullOrEmpty(Request.FromZone))
        {
            request.FromZone = string.Empty;
        }
        else
        {
            request.FromZone = Request.FromZone;
        }

        if (string.IsNullOrEmpty(Request.ToZone))
        {
            request.ToZone = string.Empty;
        }
        else
        {
            request.ToZone = Request.ToZone;
        }

        if (string.IsNullOrEmpty(Request.Driver))
        {
            request.Driver = string.Empty;
        }
        else
        {
            request.Driver = Request.Driver;
        }

        if (string.IsNullOrEmpty(Request.MissionType))
        {
            request.MissionType = string.Empty;
        }
        else
        {
            request.MissionType = Request.MissionType;
        }

        if (string.IsNullOrEmpty(Request.MissionCode))
        {
            request.MissionCode = string.Empty;
        }
        else
        {
            request.MissionCode = Request.MissionCode;
        }

        if (string.IsNullOrEmpty(Request.ProductStatus))
        {
            request.ProductStatus = string.Empty;
        }
        else
        {
            request.ProductStatus = Request.ProductStatus;
        }

        if (string.IsNullOrEmpty(Request.MissionStatus))
        {
            request.MissionStatus = string.Empty;
        }
        else
        {
            request.MissionStatus = Request.MissionStatus;
        }

        return request;
    }
}
